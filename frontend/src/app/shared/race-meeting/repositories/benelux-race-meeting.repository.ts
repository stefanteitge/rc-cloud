import { Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { RaceDateDto, RacePageDto } from '../dtos/race-date.dto';
import { RaceMeetingEnvelopeDto } from '../dtos/race-meeting-envelope.dto';
import compileUpcomingDates from '../services/compile-upcoming-dates.service';
import { FEATURE_FUNCTION_API_RACES, FeatureFlagService } from '../../feature-managment/services/feature-flag.service';
import { environment } from '../../../../environments/environment';
import {getOvalRaces} from '../../bangerdates/races';
import {getOvalClassName} from '../../bangerdates/classes';
import {Temporal} from 'temporal-polyfill';

@Injectable()
export class BeneluxRaceMeetingRepository {
  private jsonUrl = 'assets/benelux.json';
  private publicApiUrl = environment.apiRoot + 'benelux';
  public raceDates = signal<RaceDateDto[]>([]);
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
            const dates = addOvalDates(page.dates)
            this.raceDates.set(dates);
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
            const compiled = compileUpcomingDates(
              envelope.races,
              ['global', 'be', 'nl', 'lux', 'banger', 'stockcars'],
              true
            );
            this.raceDates.set(compiled);
            this.lastUpdate.set(envelope.lastUpdate);
          },
          error: err => {
            this.error.set(err.message || 'An error occurred.');
          },
        });
    }
  }
}
function addOvalDates(dates: RaceDateDto[]): RaceDateDto[] {
  const ovalRaces = getOvalRaces();

  ovalRaces.forEach(ovalRace => {
    const now = Temporal.Now.plainDateISO();
    const dur = ovalRace.date.until(now);
    if (dur.days > 0) {
      return;
    }

    var myDate = dates.find(d => d.dateEnd == ovalRace.date.toString());

    if (!myDate) {
      myDate = {
        dateEnd: ovalRace.date.toString(),
        categories: [],
      };
      dates.push(myDate);
    }

    var bangerCat = myDate.categories.find(c => c.key == 'banger');
    if (!bangerCat) {
      bangerCat = {
        key: 'banger',
        races: []
      };

      myDate.categories.push(bangerCat);
    }

    bangerCat.races.push({
      countryCode: "?",
      location: ovalRace.location,
      series: [],
      groups: [],
      source: 'Facebook groups',
      title: ovalRace.classes.map(c => getOvalClassName(c)).join(', ') + " in " + ovalRace.location
    });
  })

  return dates.sort((a, b) => a.dateEnd.localeCompare(b.dateEnd));
}

