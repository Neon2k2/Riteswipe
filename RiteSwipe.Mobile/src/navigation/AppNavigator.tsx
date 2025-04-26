import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { createBottomTabNavigator } from '@react-navigation/bottom-tabs';
import { useSelector } from 'react-redux';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

import { RootState } from '@/store';
import { colors } from '@/theme';

// Auth Screens
import LoginScreen from '@/screens/auth/LoginScreen';
import RegisterScreen from '@/screens/auth/RegisterScreen';

// Main Screens
import TaskSwipeScreen from '@/screens/tasks/TaskSwipeScreen';
import TaskDetailsScreen from '@/screens/tasks/TaskDetailsScreen';
import CreateTaskScreen from '@/screens/tasks/CreateTaskScreen';
import ProfileScreen from '@/screens/profile/ProfileScreen';
import EditProfileScreen from '@/screens/profile/EditProfileScreen';
import NotificationsScreen from '@/screens/notifications/NotificationsScreen';
import MyTasksScreen from '@/screens/tasks/MyTasksScreen';

const Stack = createNativeStackNavigator();
const Tab = createBottomTabNavigator();

const TaskStack = () => (
  <Stack.Navigator>
    <Stack.Screen 
      name="TaskSwipe" 
      component={TaskSwipeScreen}
      options={{ headerShown: false }}
    />
    <Stack.Screen 
      name="TaskDetails" 
      component={TaskDetailsScreen}
      options={{ title: 'Task Details' }}
    />
    <Stack.Screen 
      name="CreateTask" 
      component={CreateTaskScreen}
      options={{ title: 'Create Task' }}
    />
  </Stack.Navigator>
);

const ProfileStack = () => (
  <Stack.Navigator>
    <Stack.Screen 
      name="Profile" 
      component={ProfileScreen}
      options={{ headerShown: false }}
    />
    <Stack.Screen 
      name="EditProfile" 
      component={EditProfileScreen}
      options={{ title: 'Edit Profile' }}
    />
  </Stack.Navigator>
);

const TabNavigator = () => (
  <Tab.Navigator
    screenOptions={{
      tabBarActiveTintColor: colors.primary,
      tabBarInactiveTintColor: colors.textLight,
      tabBarStyle: {
        borderTopWidth: 1,
        borderTopColor: colors.border,
        height: 60,
        paddingBottom: 8,
        paddingTop: 8
      }
    }}
  >
    <Tab.Screen
      name="Tasks"
      component={TaskStack}
      options={{
        headerShown: false,
        tabBarIcon: ({ color, size }) => (
          <Icon name="cards" size={size} color={color} />
        )
      }}
    />
    <Tab.Screen
      name="MyTasks"
      component={MyTasksScreen}
      options={{
        title: 'My Tasks',
        tabBarIcon: ({ color, size }) => (
          <Icon name="format-list-checks" size={size} color={color} />
        )
      }}
    />
    <Tab.Screen
      name="Notifications"
      component={NotificationsScreen}
      options={{
        tabBarIcon: ({ color, size }) => (
          <Icon name="bell" size={size} color={color} />
        )
      }}
    />
    <Tab.Screen
      name="ProfileTab"
      component={ProfileStack}
      options={{
        title: 'Profile',
        headerShown: false,
        tabBarIcon: ({ color, size }) => (
          <Icon name="account" size={size} color={color} />
        )
      }}
    />
  </Tab.Navigator>
);

const AppNavigator = () => {
  const { token } = useSelector((state: RootState) => state.auth);

  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {!token ? (
          // Auth Stack
          <>
            <Stack.Screen name="Login" component={LoginScreen} />
            <Stack.Screen name="Register" component={RegisterScreen} />
          </>
        ) : (
          // Main App
          <Stack.Screen name="MainApp" component={TabNavigator} />
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default AppNavigator;
