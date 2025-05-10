import { Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';

export const FEATURE_FUNCTION_API_RACES = 'functions-api-races';

@Injectable({
  providedIn: 'root',
})
export class FeatureFlagService {
  isEnabled(key: string): boolean {
    if (key === FEATURE_FUNCTION_API_RACES) {
      return environment.functionsApiRaces;
    }

    return false;
  }
}
