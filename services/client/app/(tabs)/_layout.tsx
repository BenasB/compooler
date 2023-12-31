import React from "react";
import { Redirect, Tabs } from "expo-router";
import { Foundation } from "@expo/vector-icons";
import { usePublicAuthContext } from "../../hooks/auth";

const Layout = () => {
  const authState = usePublicAuthContext();

  if (authState.state !== "loggedIn") return <Redirect href="/login" />;

  return (
    <Tabs>
      <Tabs.Screen
        name="home"
        options={{
          tabBarLabel: "Home",
          tabBarIcon: () => <Foundation name="home" size={24} color="black" />,
        }}
      />
      <Tabs.Screen
        name="list"
        options={{
          tabBarLabel: "List",
          tabBarIcon: () => (
            <Foundation name="list-bullet" size={24} color="black" />
          ),
        }}
      />
      <Tabs.Screen
        name="profile"
        options={{
          tabBarLabel: "Profile",
          tabBarIcon: () => <Foundation name="torso" size={24} color="black" />,
        }}
      />
    </Tabs>
  );
};

export default Layout;
