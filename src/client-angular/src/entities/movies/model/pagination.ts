export interface PaginationWrapper<T> {
  items: T[];
  limit: number;
  offset: number;
  total: number;
  nextRef: string;
  prevRef: string;
}

export interface MoviesPaginationPayload {
  limit: number;
  offset: number;
  filters: string[];
  filterValues: string[];
  sortBy: string;
  sortDirection: "asc" | "desc";
  date: string;
}