import {Injectable, signal} from '@angular/core';
import {tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import { UpcomingDate} from '../domain/upcoming-meetings';
import {RaceMeetingEnvelopeDto} from '../dtos/race-meeting-envelope.dto';
import compileUpcomingMeetings from '../services/compile-upcoming-meetings.service';


@Injectable()
export class RaceMeetingRepository {
  private apiUrl = 'assets/all.json';
  public envelope = signal<RaceMeetingEnvelopeDto>({ retrievedDate: '', raceMeetings: [] });
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  constructor(private http: HttpClient) {
  }

  getAll(): UpcomingDate[] {
    return compileUpcomingMeetings(this.envelope().raceMeetings);
  }

  fetchAll(): void {
    this.loading.set(true);
    this.error.set(null);

    this.http.get<RaceMeetingEnvelopeDto>(this.apiUrl).pipe(
      tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
    ).subscribe({
      next: (env) => {
        this.envelope.set(env);
      },
      error: (err) => {
        this.error.set(err.message || 'An error occurred.');
      },
    });
  }

  getRetrievedDate() {
    return this.envelope().retrievedDate;
  }
}
