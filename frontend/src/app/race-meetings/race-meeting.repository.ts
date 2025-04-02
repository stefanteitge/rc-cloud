import {Injectable, signal} from '@angular/core';
import {tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {compileUpcomingMeetings, UpcomingDate, UpcomingRaceMeeting} from './upcoming-meetings.model';

export interface RaceMeetingEnvelopeDto
{
  retrievedDate: string;
  raceMeetings: RaceMeetingDto[];
}

export interface ReferenceDto {
  id: string;
}

export interface RaceMeetingDto {
  date: string;
  location: string;
  title: string
  series: ReferenceDto[];
  groups: ReferenceDto[];
}

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
