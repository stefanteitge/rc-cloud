import {Injectable, signal} from '@angular/core';
import {tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {GermanyPage, RaceDate} from '../domain/race-date';
import {RaceMeetingEnvelopeDto} from '../dtos/race-meeting-envelope.dto';
import compileUpcomingDates from '../services/compile-upcoming-dates.service';
import {FEATURE_FUNCTION_API_RACES, FeatureFlagService} from '../../feature-managment/services/feature-flag.service';
import {environment} from '../../../../environments/environment';

@Injectable()
export class RaceMeetingRepository {
  private apiGermanyUrl = 'assets/germany.json';
  private apiBeneluxUrl = 'assets/benelux.json';
  private publicApiUrl = environment.apiRoot + 'germany';
  public germany = signal<RaceDate[]>([]);
  public germanyLastUpdate = signal('');
  public benelux = signal<RaceMeetingEnvelopeDto>({ lastUpdate: '', races: [] });
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  constructor(private http: HttpClient, private featureFlags: FeatureFlagService) {
  }

  getAll(dataSet: string): RaceDate[] {
    if (dataSet == 'benelux') {
      return compileUpcomingDates(this.benelux().races, ['be', 'nl', 'lu', 'banger', 'stockcar'], true);
    }

    return this.germany();
  }

  fetchAll(): void {
    this.loading.set(true);
    this.error.set(null);

    if (this.featureFlags.isEnabled(FEATURE_FUNCTION_API_RACES)) {
      this.http.get<GermanyPage>(this.publicApiUrl).pipe(
        tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
      ).subscribe({
        next: (page) => {
          this.germany.set(page.dates);
          this.germanyLastUpdate.set(page.lastUpdate);
        },
        error: (err) => {
          this.error.set(err.message || 'An error occurred.');
        },
      });
    } else {
      this.http.get<RaceMeetingEnvelopeDto>(this.apiGermanyUrl).pipe(
        tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
      ).subscribe({
        next: (envelope) => {
          const compiled = compileUpcomingDates(envelope.races, ['east', 'west', 'north', 'south', 'central'])
          this.germany.set(compiled);
          this.germanyLastUpdate.set(envelope.lastUpdate);
        },
        error: (err) => {
          this.error.set(err.message || 'An error occurred.');
        },
      });
    }

    this.http.get<RaceMeetingEnvelopeDto>(this.apiBeneluxUrl).pipe(
      tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
    ).subscribe({
      next: (env) => {
        this.benelux.set(env);
      },
      error: (err) => {
        this.error.set(err.message || 'An error occurred.');
      },
    });
  }

  getRetrievedDate(dataSet: string) {
    if (dataSet == 'benelux') {
      return this.benelux().lastUpdate;
    }

    return this.germanyLastUpdate();
  }
}
