import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule],
  template: `
    <mat-toolbar color="primary">
      <div class="brand-nav">
        <a routerLink="/" mat-button class="brand-link">
          <span class="brand-main">RC Cloud</span> <span class="brand-sub">Termine</span>
        </a>
        <span class="divider"></span>
        <a routerLink="germany" mat-button class="nav-link" routerLinkActive="nav-link-active">Deutschland</a>
        <a routerLink="benelux" mat-button class="nav-link" routerLinkActive="nav-link-active">BeNeLux</a>
        <a routerLink="clubs" mat-button class="nav-link" routerLinkActive="nav-link-active">Clubs</a>
      </div>
      <span class="spacer"></span>
    </mat-toolbar>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    .brand-nav { display: flex; align-items: baseline; gap: 4px; }
    .brand-link { padding: 0 8px; line-height: normal; }
    .brand-main { font-size: 1.25rem; font-weight: 700; letter-spacing: 0.02em; margin-right: 0.2em; }
    .brand-sub { font-size: 0.85rem; font-weight: 400; opacity: 0.85; }
    .divider { width: 1px; height: 1.2em; background: rgba(255,255,255,0.35); margin: 0 4px; align-self: center; }
    .nav-link { font-size: 0.875rem; font-weight: 400; opacity: 0.85; letter-spacing: 0.04em; text-transform: uppercase; line-height: normal; }
    .nav-link:hover { opacity: 1; }
    .nav-link-active { opacity: 1; font-weight: 600; }
    mat-toolbar { position: sticky; top: 0; z-index: 1000; }
  `]
})
export class AppComponent {}

