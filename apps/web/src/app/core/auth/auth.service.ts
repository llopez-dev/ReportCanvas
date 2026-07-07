import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, UserDto } from './auth.models';

const TOKEN_KEY = 'rc_token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  // Signals — source of truth for auth state
  private readonly _token = signal<string | null>(localStorage.getItem(TOKEN_KEY));
  private readonly _user = signal<UserDto | null>(null);

  readonly isAuthenticated = computed(() => !!this._token());
  readonly currentUser = this._user.asReadonly();
  readonly token = this._token.asReadonly();

  login(request: LoginRequest) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap(res => this.setSession(res))
    );
  }

  register(request: RegisterRequest) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/register`, request).pipe(
      tap(res => this.setSession(res))
    );
  }

  logout(): void {
    localStorage.removeItem(TOKEN_KEY);
    this._token.set(null);
    this._user.set(null);
    this.router.navigate(['/auth/login']);
  }

  private setSession(res: AuthResponse): void {
    localStorage.setItem(TOKEN_KEY, res.accessToken);
    this._token.set(res.accessToken);
    this._user.set(res.user);
  }
}
