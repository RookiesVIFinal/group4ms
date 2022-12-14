import { useRouteError } from "react-router-dom";

export function ErrorPage() {
  const error = useRouteError();

  return (
    <h1>
      <strong>[{error.status}]</strong> {error.statusText || error.message}
    </h1>
  );
}
