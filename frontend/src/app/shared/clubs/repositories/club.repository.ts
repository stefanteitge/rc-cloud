import {Injectable, signal} from '@angular/core';
import {tap} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {environment} from '../../../../environments/environment';
import {ClubDto} from '../dtos/club.dto';

@Injectable({
  providedIn: 'root',
})
export class ClubRepository {
  private publicApiUrl = environment.apiRoot + 'clubs';
  public clubs = signal<ClubDto[]>([]);
  public loading = signal<boolean>(false);
  public error = signal<string | null>(null);

  constructor(private http: HttpClient) {
  }

  fetchAll(): void {
    this.loading.set(true);
    this.error.set(null);

    this.http.get<ClubDto[]>(this.publicApiUrl).pipe(
      tap(() => this.loading.set(false)), // Set loading to false after successful or failed request
    ).subscribe({
      next: (clubs) => {
        this.clubs.set(clubs);
      },
      error: (err) => {
        this.error.set(err.message || 'An error occurred.');
      },
    });
  }
}
