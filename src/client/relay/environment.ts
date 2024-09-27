import Constants from "expo-constants";
import {
  Store,
  RecordSource,
  Environment,
  Network,
  Observable,
} from "relay-runtime";
import type { FetchFunction, IEnvironment } from "relay-runtime";

const testToken =
  "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJ1c2VyX2lkIjoiYmVsZW5rYXMifQ.-XrnAD8NrKOv4rt3mgOn3VXPcr2pSzA9FOEmCosItJE";

const fetchFn: FetchFunction = (params, variables) => {
  var host = Constants?.expoConfig?.hostUri?.split(`:`).shift() || "";
  const response = fetch(`http://${host}:5121/graphql`, {
    method: "POST",
    headers: [
      ["Content-Type", "application/json"],
      ["Authorization", `Bearer ${testToken}`],
    ],
    body: JSON.stringify({
      query: params.text,
      variables,
    }),
  });

  return Observable.from(response.then((data) => data.json()));
};

export function createEnvironment(): IEnvironment {
  const network = Network.create(fetchFn);
  const store = new Store(new RecordSource());
  return new Environment({ store, network });
}
