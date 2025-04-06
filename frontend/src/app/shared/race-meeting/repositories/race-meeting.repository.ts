import {Injectable, signal} from '@angular/core';
import {tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {UpcomingDate} from '../domain/upcoming-date';
import {RaceMeetingEnvelopeDto} from '../dtos/race-meeting-envelope.dto';
import compileUpcomingDates from '../services/compile-upcoming-dates.service';

@Injectable()
export class RaceMeetingRepository {
  private apiGermanyUrl = 'assets/germany.json';
  private apiBeneluxUrl = 'assets/benelux.json';
  public germany = signal<RaceMeetingEnvelopeDto>({ retrievedDate: '', raceMeetings: [] });
  public benelux = signal<RaceMeetingEnvelopeDto>({ retrievedDate: '', raceMeetings: [] });
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  constructor(private http: HttpClient) {
  }

  getAll(dataSet: string): UpcomingDate[] {
    if (dataSet == 'benelux') {
      return compileUpcomingDates(this.benelux().raceMeetings, ['be', 'nl', 'lux', 'banger', 'stockcar']);
    }

    return compileUpcomingDates(this.germany().raceMeetings, ['east', 'west', 'north', 'south', 'central']);
  }

  fetchAll(): void {
    this.loading.set(true);
    this.error.set(null);

    this.http.get<RaceMeetingEnvelopeDto>(this.apiGermanyUrl).pipe(
      tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
    ).subscribe({
      next: (env) => {
        this.germany.set(env);
      },
      error: (err) => {
        this.error.set(err.message || 'An error occurred.');
      },
    });

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
      return this.benelux().retrievedDate;
    }

    return this.germany().retrievedDate;
  }
}
