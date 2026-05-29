import React from 'react';

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
        <button
          disabled={!hasPreviousPage}
          onClick={() => onPageChange(page - 1)}
          className="px-3 py-1 border rounded disabled:opacity-50 hover:bg-gray-50"
        >
          Previous
        </button>
        <button
          disabled={!hasNextPage}
          onClick={() => onPageChange(page + 1)}
          className="px-3 py-1 border rounded disabled:opacity-50 hover:bg-gray-50"
        >
          Next
        </button>
      </div>
    </div>
  );
}
