import { Component, computed, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';

interface ChassisPreset {
  name: string;
  ratio: number;
}

interface GearCell {
  ratio: number;
  isExact: boolean;
  isClosest: boolean;
  isInTolerance: boolean;
}

interface BestCombination {
  pinion: number;
  spur: number;
  ratio: number;
  isExact: boolean;
  isClosest: boolean;
  isInTolerance: boolean;
}

/** One row in the matrix: pinion value + one cell per spur column */
interface MatrixRow {
  pinion: number;
  cells: Record<number, GearCell>;
}

const CHASSIS_PRESETS: ChassisPreset[] = [{ name: 'Tamiya TT-02', ratio: 2.6 }];

@Component({
  selector: 'app-gearing-calculator',
  imports: [
    DecimalPipe,
    FormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatTableModule,
  ],
  templateUrl: './gearing-calculator.component.html',
  styleUrl: './gearing-calculator.component.scss',
})
export class GearingCalculatorComponent {
  readonly presets = CHASSIS_PRESETS;

  spurMin = signal(60);
  spurMax = signal(90);
  pinionMin = signal(20);
  pinionMax = signal(40);
  internalRatio = signal(2.6);
  targetRatio = signal(6.0);
  tolerance = signal(0.05);
  selectedPreset = signal<ChassisPreset | null>(CHASSIS_PRESETS[0]);

  /** Spur values used as dynamic column keys, e.g. ['spur-80', 'spur-81', ...] */
  spurValues = computed<number[]>(() => {
    const sMin = this.spurMin();
    const sMax = this.spurMax();
    if (sMin > sMax) return [];
    return Array.from({ length: sMax - sMin + 1 }, (_, i) => sMin + i);
  });

  displayedColumns = computed<string[]>(() => [
    'pinion',
    ...this.spurValues().map((s) => `spur-${s}`),
  ]);

  matrix = computed<MatrixRow[]>(() => {
    const spurVals = this.spurValues();
    const pMin = this.pinionMin();
    const pMax = this.pinionMax();
    const internal = this.internalRatio();
    const target = this.targetRatio();
    const tolerance = this.tolerance();

    if (spurVals.length === 0 || pMin > pMax || internal <= 0) return [];

    // Build matrix and track closest ratio
    let minDiff = Infinity;
    const rows: MatrixRow[] = [];

    for (let pinion = pMin; pinion <= pMax; pinion++) {
      const cells: Record<number, GearCell> = {};
      for (const spur of spurVals) {
        const ratio = (spur / pinion) * internal;
        const diff = Math.abs(ratio - target);
        if (diff < minDiff) minDiff = diff;
        cells[spur] = { ratio, isExact: false, isClosest: false, isInTolerance: false };
      }
      rows.push({ pinion, cells });
    }

    // Mark exact, closest and tolerance cells
    const epsilon = 1e-9;
    for (const row of rows) {
      for (const spur of spurVals) {
        const cell = row.cells[spur];
        cell.isExact = Math.abs(cell.ratio - target) < epsilon;
        cell.isClosest = Math.abs(Math.abs(cell.ratio - target) - minDiff) < epsilon;
        cell.isInTolerance = cell.ratio >= target && cell.ratio <= target + tolerance;
      }
    }

    return rows;
  });

  bestCombinations = computed<BestCombination[]>(() => {
    const rows = this.matrix();
    const spurVals = this.spurValues();
    const results: BestCombination[] = [];
    for (const row of rows) {
      for (const spur of spurVals) {
        const cell = row.cells[spur];
        if (cell.isClosest || cell.isInTolerance) {
          results.push({ pinion: row.pinion, spur, ratio: cell.ratio, isExact: cell.isExact, isClosest: cell.isClosest, isInTolerance: cell.isInTolerance });
        }
      }
    }
    return results.sort((a, b) => a.ratio - b.ratio);
  });

  showTable = signal(false);

  onPresetChange(preset: ChassisPreset | null): void {
    if (preset) {
      this.internalRatio.set(preset.ratio);
    }
  }

  onInternalRatioChange(value: number): void {
    this.selectedPreset.set(null);
    this.internalRatio.set(value);
  }
}
