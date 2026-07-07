import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export interface DatasetColumn {
  name: string;
  inferredType: string;
  columnIndex: number;
}

export interface DatasetPreview {
  id: string;
  name: string;
  originalFileName: string;
  fileType: string;
  fileSizeBytes: number;
  rowCount: number;
  columnCount: number;
  columns: DatasetColumn[];
  previewRows: string[][];
}

export interface DatasetSummary {
  id: string;
  name: string;
  originalFileName: string;
  fileType: string;
  fileSizeBytes: number;
  rowCount: number;
  columnCount: number;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class DatasetService {
  private readonly http = inject(HttpClient);

  getAll(workspaceId: string) {
    return this.http.get<DatasetSummary[]>(
      `${environment.apiUrl}/workspaces/${workspaceId}/datasets`
    );
  }

  getById(workspaceId: string, datasetId: string) {
    return this.http.get<DatasetPreview>(
      `${environment.apiUrl}/workspaces/${workspaceId}/datasets/${datasetId}`
    );
  }

  upload(workspaceId: string, file: File, name?: string) {
    const form = new FormData();
    form.append('file', file);
    if (name) form.append('name', name);

    return this.http.post<DatasetPreview>(
      `${environment.apiUrl}/workspaces/${workspaceId}/datasets`,
      form
    );
  }
}
