import React from 'react';
import { Button } from '@/components/ui/Button';

interface PaginationProps {
  page: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
  onPageChange: (newPage: number) => void;
}

export function Pagination({ page, totalPages, hasNextPage, hasPreviousPage, onPageChange }: PaginationProps) {
  return (
    <div className="flex justify-between items-center mt-6">
      <div className="text-sm text-gray-500">
        Page {page} of {totalPages === 0 ? 1 : totalPages}
      </div>
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
  );
}
