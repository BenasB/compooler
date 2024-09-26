import ContentContainer from "@/components/layout/ContentContainer";
import { ThemedText } from "@/components/ThemedText";
import { Link } from "expo-router";

export default function Me() {
  return (
    <ContentContainer>
      <ThemedText>Me.</ThemedText>
      <Link href="https://github.com/benasb/compooler">
        <ThemedText type="link">Click me</ThemedText>
      </Link>
    </ContentContainer>
  );
}
