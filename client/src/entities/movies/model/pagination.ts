export interface PaginationWrapper<T> {
  items: T[];
  limit: number;
  offset: number;
  total: number;
	nextRef: string,
	prevRef: string
}

export interface PaginationPayload {
  limit: number;
  offset: number;
  filter: string;
  filterValue: string;
  sortBy: string;
  sortDirection: "asc" | "desc";
}