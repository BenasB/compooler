/**
 * @generated SignedSource<<d73d2644d71ac16a9b01c326cb652861>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type meDetailsQuery$variables = Record<PropertyKey, never>;
export type meDetailsQuery$data = {
  readonly me: {
    readonly firstName: string;
    readonly id: string;
    readonly lastName: string;
  } | null | undefined;
};
export type meDetailsQuery = {
  response: meDetailsQuery$data;
  variables: meDetailsQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "User",
    "kind": "LinkedField",
    "name": "me",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "id",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "firstName",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "lastName",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "meDetailsQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "meDetailsQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "c578a68f441be20c0f7b46c5d67d888a",
    "id": null,
    "metadata": {},
    "name": "meDetailsQuery",
    "operationKind": "query",
    "text": "query meDetailsQuery {\n  me {\n    id\n    firstName\n    lastName\n  }\n}\n"
  }
};
})();

(node as any).hash = "dc7a9b78f2d3ba84ff436bb5feb2433d";

export default node;
