import { meDetailsQuery as meDetailsQueryType } from "@/__generated__/meDetailsQuery.graphql";
import ContentContainer from "@/components/layout/ContentContainer";
import { ThemedText } from "@/components/ThemedText";
import { Link } from "expo-router";
import { useLazyLoadQuery } from "react-relay";
import { graphql } from "relay-runtime";

const meDetailsQuery = graphql`
  query meDetailsQuery {
    me {
      id
      firstName
      lastName
    }
  }
`;

export default function Me() {
  const data = useLazyLoadQuery<meDetailsQueryType>(meDetailsQuery, {});

  return (
    <ContentContainer>
      <ThemedText>Me</ThemedText>
      {data.me ? (
        <>
          <ThemedText type="subtitle">{data.me?.id}</ThemedText>
          <ThemedText>{data.me?.firstName}</ThemedText>
          <ThemedText>{data.me?.lastName}</ThemedText>
        </>
      ) : (
        <ThemedText>I was not found :/</ThemedText>
      )}

      <Link href="https://github.com/benasb/compooler">
        <ThemedText type="link">Click me</ThemedText>
      </Link>
    </ContentContainer>
  );
}
