import { Routes } from '@angular/router';

export const DATASETS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/datasets-list.component').then(m => m.DatasetsListComponent)
  }
];
