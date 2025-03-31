import {Injectable, signal} from '@angular/core';
import {tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';

export interface Reference {
  id: string;
}

export interface RaceMeeting {
  date: string;
  location: string;
  series: Reference;
  groups: Reference[];
}

@Injectable()
export class RaceMeetingRepository {
  private apiUrl = 'assets/all.json';
  public meetings = signal<RaceMeeting[]>([]);
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  constructor(private http: HttpClient) {
  }

  getAll(): RaceMeeting[] {
    return this.meetings();
  }

  fetchAll(): void {
    this.loading.set(true);
    this.error.set(null);

    this.http.get<RaceMeeting[]>(this.apiUrl).pipe(
      tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
    ).subscribe({
      next: (todos) => {
        this.meetings.set(todos);
      },
      error: (err) => {
        this.error.set(err.message || 'An error occurred.');
      },
    });
  }
}
