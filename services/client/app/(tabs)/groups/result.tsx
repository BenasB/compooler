import {
  Center,
  RefreshControl,
  ScrollView,
  Spinner,
  Text,
  VStack,
  SafeAreaView,
  Button,
  ButtonText,
} from "@gluestack-ui/themed";
import React, { useCallback, useState } from "react";
import GroupInformation from "../../../components/groups/GroupInformation";
import { useMutation, useQuery } from "@apollo/client";
import { usePrivateAuthContext } from "../../../hooks/auth";
import {
  Stack,
  router,
  useFocusEffect,
  useLocalSearchParams,
} from "expo-router";
import { gql } from "../../../__generated__";
import { Clients } from "../../../hooks/apollo";

const GET_JOINABLE_GROUPS = gql(`
  query GetNearestJoinableGroups(
    $userStartLocation: CoordinatesInput!
    $userEndLocation: CoordinatesInput!
    $currentUserId: String!
  ) {
    nearestGroups(
      userStartLocation: $userStartLocation
      userEndLocation: $userEndLocation
      where: {
        and: [
          { driver: { id: { neq: $currentUserId } } }
          { passengers: { none: { id: { eq: $currentUserId } } } }
        ]
      }
    ) {
      id
      startTime
      days
      startLocation {
        latitude
        longitude
        distance(to: $userStartLocation)
      }
      endLocation {
        latitude
        longitude
        distance(to: $userEndLocation)
      }
      totalSeats
      passengers {
        id
      }
    }
  }
`);

const JOIN_GROUP = gql(`
  mutation JoinGroup($groupId: Int!){
    joinGroup(input: {id: $groupId}){
      group {
        id
      }
      errors {
        ... on Error{
          message
        }
      }
    } 
  }
`);

const JOIN_UPCOMING_RIDES = gql(`
  mutation JoinUpcomingRides($groupId: Int!){
    joinUpcomingRides(input: { groupId: $groupId }) {
      ride {
        id
      }
      errors {
        ... on Error {
          message
        }
      }
    }
  }
`);

const Join = () => {
  const queryParameters = useLocalSearchParams<{
    startLatitude: string;
    startLongitude: string;
    endLatitude: string;
    endLongitude: string;
  }>();
  const { user } = usePrivateAuthContext();

  if (
    !queryParameters.startLatitude ||
    isNaN(+queryParameters.startLatitude) ||
    !queryParameters.startLongitude ||
    isNaN(+queryParameters.startLongitude) ||
    !queryParameters.endLatitude ||
    isNaN(+queryParameters.endLatitude) ||
    !queryParameters.endLongitude ||
    isNaN(+queryParameters.endLongitude)
  )
    return <Text>Unexpected parameters!</Text>;

  const userStartLocation = {
    latitude: +queryParameters.startLatitude,
    longitude: +queryParameters.startLongitude,
  };

  const userEndLocation = {
    latitude: +queryParameters.endLatitude,
    longitude: +queryParameters.endLongitude,
  };

  const {
    loading: queryLoading,
    error: queryError,
    data: queryData,
    refetch,
  } = useQuery(GET_JOINABLE_GROUPS, {
    variables: {
      userStartLocation: userStartLocation,
      userEndLocation: userEndLocation,
      currentUserId: user.uid,
    },
    notifyOnNetworkStatusChange: true,
    context: {
      clientName: Clients.GroupMaker,
    },
  });

  const [
    joinGroupFunction,
    {
      data: joinGroupData,
      loading: joinGroupLoading,
      error: joinGroupError,
      called: joinGroupCalled,
    },
  ] = useMutation(JOIN_GROUP, {
    onCompleted: (data) => {
      if (data.joinGroup.errors || !data.joinGroup.group) return;

      // TODO: Data inconsistency can arise here
      joinRidesFunction({ variables: { groupId: data.joinGroup.group.id } });
    },
    context: {
      clientName: Clients.GroupMaker,
    },
  }); // Don't need to refetch GET_JOINABLE_GROUPS since we'll be navigated off this page

  const [
    joinRidesFunction,
    {
      data: joinRidesData,
      loading: joinRidesLoading,
      error: joinRidesError,
      called: joinRidesCalled,
    },
  ] = useMutation(JOIN_UPCOMING_RIDES, {
    onCompleted: (data) => {
      if (data.joinUpcomingRides.errors) return;

      router.navigate("/groups");
    },
    context: {
      clientName: Clients.Rides,
    },
  });

  useFocusEffect(
    useCallback(() => {
      refetch();
    }, [])
  );

  const [refreshing, setRefreshing] = useState<boolean>(false);

  const onRefresh = useCallback(async () => {
    setRefreshing(true);
    await refetch();
    setRefreshing(false);
  }, []);

  const body =
    queryLoading || joinGroupLoading || joinRidesLoading ? (
      <Spinner />
    ) : queryError || queryData === undefined ? (
      <Text>Whoops! Ran into an error :/</Text>
    ) : joinGroupCalled && (joinGroupError || joinGroupData === undefined) ? (
      <Text>Whoops! Ran into an error when joining a group :/</Text>
    ) : joinRidesCalled && (joinRidesError || joinRidesData === undefined) ? (
      <Text>
        Whoops! Ran into an error when joining the group's upcoming rides :/
      </Text>
    ) : joinGroupData?.joinGroup.errors ? (
      <>
        <Text textAlign="center">Ran into an error!</Text>
        {joinGroupData.joinGroup.errors.map((x) => (
          <Text key={x.message} textAlign="center">
            {x.message}
          </Text>
        ))}
      </>
    ) : joinRidesData?.joinUpcomingRides.errors ? (
      <>
        <Text textAlign="center">Ran into an error!</Text>
        {joinRidesData.joinUpcomingRides.errors.map((x) => (
          <Text key={x.message} textAlign="center">
            {x.message}
          </Text>
        ))}
      </>
    ) : queryData.nearestGroups.length === 0 ? (
      <Text color="$secondary400" textAlign="center">
        Seems like there are no new groups you can join :/
      </Text>
    ) : (
      <VStack space="md">
        {queryData.nearestGroups.map((group) => (
          <GroupInformation
            key={group.id}
            group={{
              startTime: group.startTime
                .replace(/[PTM]/g, "")
                .split("H")
                .map((x) => x.padStart(2, "0"))
                .join(":"),
              days: group.days,
              startLocation: group.startLocation,
              endLocation: group.endLocation,
              seats: {
                total: group.totalSeats,
                occupied: group.passengers.length + 1,
              },
            }}
            button={
              <Button
                variant="outline"
                action="positive"
                onPress={async () => {
                  await joinGroupFunction({
                    variables: {
                      groupId: group.id,
                    },
                  });
                }}
              >
                <ButtonText>Join</ButtonText>
              </Button>
            }
          />
        ))}
      </VStack>
    );

  return (
    <SafeAreaView h="$full">
      <Stack.Screen options={{ title: "Join a group" }} />
      <ScrollView
        refreshControl={
          <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
        }
      >
        <Center h="$full" p="$5">
          <VStack
            space="md"
            h="$full"
            $base-w={"100%"}
            $md-w={"60%"}
            $lg-w={550}
          >
            {body}
          </VStack>
        </Center>
      </ScrollView>
    </SafeAreaView>
  );
};

export default Join;
