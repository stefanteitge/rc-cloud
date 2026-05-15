import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive, MatToolbarModule, MatButtonModule, MatMenuModule, MatIconModule],
  template: `
    <mat-toolbar color="primary">
      <div class="brand-nav">
        <a routerLink="/" mat-button class="brand-link">
          <span class="brand-main">RC Cloud</span> <span class="brand-sub">Hub</span>
        </a>

        <!-- Desktop nav links -->
        <span class="divider nav-desktop"></span>
        <a routerLink="germany" mat-button class="nav-link nav-desktop" routerLinkActive="nav-link-active">Germany</a>
        <a routerLink="benelux" mat-button class="nav-link nav-desktop" routerLinkActive="nav-link-active">BeNeLux</a>
        <a routerLink="clubs" mat-button class="nav-link nav-desktop" routerLinkActive="nav-link-active">Clubs</a>
        <a routerLink="tools/gearing-calculator" mat-button class="nav-link nav-desktop" routerLinkActive="nav-link-active">Gearing Calculator</a>

        <!-- Mobile hamburger -->
        <button mat-icon-button class="nav-mobile-btn" [matMenuTriggerFor]="mobileMenu" aria-label="Navigation menu">
          <mat-icon>menu</mat-icon>
        </button>
        <mat-menu #mobileMenu="matMenu">
          <a mat-menu-item routerLink="germany">Germany</a>
          <a mat-menu-item routerLink="benelux">BeNeLux</a>
          <a mat-menu-item routerLink="clubs">Clubs</a>
          <a mat-menu-item routerLink="tools/gearing-calculator">Gearing Calculator</a>
        </mat-menu>
      </div>
      <span class="spacer"></span>
    </mat-toolbar>
    <router-outlet></router-outlet>
  `,
  styles: [`
    .spacer { flex: 1 1 auto; }
    .brand-nav { display: flex; align-items: center; gap: 4px; }
    .brand-link { padding: 0 8px; line-height: normal; }
    .brand-main { font-size: 1.25rem; font-weight: 700; letter-spacing: 0.02em; margin-right: 0.2em; }
    .brand-sub { font-size: 0.85rem; font-weight: 400; opacity: 0.85; }
    .divider { width: 1px; height: 1.2em; background: rgba(255,255,255,0.35); margin: 0 4px; align-self: center; }
    .nav-link { font-size: 0.875rem; font-weight: 400; opacity: 0.85; letter-spacing: 0.04em; text-transform: uppercase; line-height: normal; }
    .nav-link:hover { opacity: 1; }
    .nav-link-active { opacity: 1; font-weight: 600; }
    mat-toolbar { position: sticky; top: 0; z-index: 1000; }
    .menu-arrow { font-size: 18px; height: 18px; width: 18px; vertical-align: middle; }
    .nav-mobile-btn { display: none; }
    @media (max-width: 559px) {
      .nav-desktop { display: none !important; }
      .nav-mobile-btn { display: inline-flex; }
    }
  `]
})
export class AppComponent {}

