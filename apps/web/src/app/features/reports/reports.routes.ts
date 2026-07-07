import { Routes } from '@angular/router';

export const REPORTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/reports-list.component').then(m => m.ReportsListComponent)
  },
  {
    path: ':reportId/editor',
    loadComponent: () =>
      import('./editor/report-editor.component').then(m => m.ReportEditorComponent)
  }
];
