import React from 'react';
import { Button } from '@/components/ui/Button';

interface PaginationProps {
  page: number;
  pageSize?: number;
  totalCount?: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  onPageChange: (newPage: number) => void;
  onPageSizeChange?: (newPageSize: number) => void;
}

export function Pagination({ page, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage, onPageChange, onPageSizeChange }: PaginationProps) {
  return (
    <div className="flex flex-col sm:flex-row justify-between items-center mt-6 gap-4">
      <div className="text-sm text-gray-500 flex items-center space-x-4">
        <span>
          Page {page} of {totalPages === 0 ? 1 : totalPages}
        </span>
        {totalCount !== undefined && (
          <span>
            Total Records: <span className="font-medium text-slate-700">{totalCount}</span>
          </span>
        )}
      </div>

      <div className="flex items-center space-x-4">
        {onPageSizeChange && pageSize !== undefined && (
          <div className="flex items-center space-x-2 text-sm text-gray-500">
            <span>Show:</span>
            <select
              value={pageSize}
              onChange={(e) => onPageSizeChange(Number(e.target.value))}
              className="border-slate-200 rounded-md text-sm focus:ring-slate-500 focus:border-slate-500"
            >
              {[5, 10, 20, 50].map((size) => (
                <option key={size} value={size}>
                  {size}
                </option>
              ))}
            </select>
          </div>
        )}

        <div className="flex space-x-2">
          <Button
            variant="secondary"
            disabled={!hasPreviousPage}
            onClick={() => onPageChange(page - 1)}
          >
            Previous
          </Button>
          <Button
            variant="secondary"
            disabled={!hasNextPage}
            onClick={() => onPageChange(page + 1)}
          >
            Next
          </Button>
        </div>
      </div>
    </div>
  );
}
