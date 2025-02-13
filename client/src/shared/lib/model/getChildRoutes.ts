import { Router } from "@angular/router";

export const getChildRoutes = (
  router: Router,
  rootPath: string,
): { route: string; title: string }[] => {
  const rootRoute = router.config.find((r) => r.path === rootPath);
	console.log(rootRoute)
  if (rootRoute && rootRoute.children) {
    return rootRoute.children
      .filter((r) => r.path && r.data?.["isVisible"])
      .map((r) => ({
        route: "/" + r.path,
        title: r.data?.["name"] ?? "Unnamed",
      }));
  }

  return [];
};
