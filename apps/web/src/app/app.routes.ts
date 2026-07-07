import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    loadChildren: () =>
      import('./features/auth/auth.routes').then(m => m.AUTH_ROUTES)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/dashboard/dashboard.routes').then(m => m.DASHBOARD_ROUTES)
  },
  {
    path: 'workspaces/:workspaceId/datasets',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/datasets/datasets.routes').then(m => m.DATASETS_ROUTES)
  },
  {
    path: 'workspaces/:workspaceId/reports',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/reports/reports.routes').then(m => m.REPORTS_ROUTES)
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
