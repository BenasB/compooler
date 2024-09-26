import ContentContainer from "@/components/layout/ContentContainer";
import { ThemedText } from "@/components/ThemedText";

export default function Index() {
  return (
    <ContentContainer>
      <ThemedText type="title">Hello world!</ThemedText>
      <ThemedText>Edit app/index.tsx to edit this screen.</ThemedText>
    </ContentContainer>
  );
}
