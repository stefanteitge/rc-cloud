import { Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { RacePageDto, RaceDate } from '../domain/race-date';
import { RaceMeetingEnvelopeDto } from '../dtos/race-meeting-envelope.dto';
import compileUpcomingDates from '../services/compile-upcoming-dates.service';
import { FEATURE_FUNCTION_API_RACES, FeatureFlagService } from '../../feature-managment/services/feature-flag.service';
import { environment } from '../../../../environments/environment';

@Injectable()
export class RaceMeetingRepository {
  private jsonUrl = 'assets/germany.json';
  private publicApiUrl = environment.apiRoot + 'germany';
  public races = signal<RaceDate[]>([]);
  public lastUpdate = signal('');
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  constructor(
    private http: HttpClient,
    private featureFlags: FeatureFlagService
  ) {}

  fetchAll(): void {
    this.loading.set(true);
    this.error.set(null);

    if (this.featureFlags.isEnabled(FEATURE_FUNCTION_API_RACES)) {
      this.http
        .get<RacePageDto>(this.publicApiUrl)
        .pipe(
          tap(() => this.loading.set(false)) // Set loading to false after successful or failed request
        )
        .subscribe({
          next: page => {
            this.races.set(page.dates);
            this.lastUpdate.set(page.lastUpdate);
          },
          error: err => {
            this.error.set(err.message || 'An error occurred.');
          },
        });
    } else {
      this.http
        .get<RaceMeetingEnvelopeDto>(this.jsonUrl)
        .pipe(
          tap(() => this.loading.set(false)) // Set loading to false after successful or failed request
        )
        .subscribe({
          next: envelope => {
            const compiled = compileUpcomingDates(envelope.races, ['east', 'west', 'north', 'south', 'central']);
            this.races.set(compiled);
            this.lastUpdate.set(envelope.lastUpdate);
          },
          error: err => {
            this.error.set(err.message || 'An error occurred.');
          },
        });
    }
  }
}
