// Import from separate modules in Redux Toolkit v2
import { fetchBaseQuery } from '@reduxjs/toolkit/dist/query';
import { createApi } from '@reduxjs/toolkit/dist/query/react';
import Config from 'react-native-config';
import type { RootState } from '../store';
import type { User, Task, Profile, Notification } from '../types';

// Import Headers type from fetch API
import type { BaseQueryApi } from '@reduxjs/toolkit/dist/query';
type FetchHeaders = Headers;

interface LoginCredentials {
  email: string;
  password: string;
}

interface RegisterData extends LoginCredentials {
  name: string;
}

interface TaskInput {
  title: string;
  description: string;
  budget: number;
  skills: string[];
  images: string[];
}

interface TaskUpdateInput extends Partial<TaskInput> {
  id: string;
}

interface ChatRoom {
  id: string;
  name: string;
  lastMessage?: string;
  lastMessageTime?: string;
  participants: User[];
}

interface ChatMessage {
  id: string;
  content: string;
  senderId: string;
  roomId: string;
  createdAt: string;
  sender?: User;
}

export const api = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: Config.API_URL || 'http://localhost:5000',
    prepareHeaders: (headers, api) => {
      const token = (api.getState() as RootState).auth.token;
      if (token) {
        headers.set('authorization', `Bearer ${token}`);
      }
      return headers;
    }
  }),
  tagTypes: ['Tasks', 'User', 'Notifications', 'Chat'] as const,
  endpoints: (builder: any) => ({
    login: builder.mutation({
      query: (credentials: LoginCredentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials
      })
    }),
    register: builder.mutation({
      query: (userData: RegisterData) => ({
        url: '/auth/register',
        method: 'POST',
        body: userData
      })
    }),
    getTasks: builder.query({
      query: () => '/tasks',
      providesTags: (result: Task[] | undefined) => ['Tasks' as const]
    }),
    createTask: builder.mutation({
      query: (task: TaskInput) => ({
        url: '/tasks',
        method: 'POST',
        body: task
      }),
      invalidatesTags: ['Tasks' as const]
    }),
    updateTask: builder.mutation({
      query: ({ id, ...task }: TaskUpdateInput) => ({
        url: `/tasks/${id}`,
        method: 'PUT',
        body: task
      }),
      invalidatesTags: ['Tasks' as const]
    }),
    getProfile: builder.query({
      query: () => '/profile',
      providesTags: (result: Profile | undefined) => ['User' as const]
    }),

    updateProfile: builder.mutation({
      query: (profile: Partial<Profile>) => ({
        url: '/users/profile',
        method: 'PUT',
        body: profile
      }),
      invalidatesTags: ['User' as const]
    }),

    getNotifications: builder.query({
      query: () => '/notifications',
      providesTags: (result: Notification[] | undefined) => ['Notifications' as const]
    }),

    markNotificationRead: builder.mutation({
      query: (id: string) => ({
        url: `/notifications/${id}/read`,
        method: 'PUT'
      }),
      invalidatesTags: ['Notifications' as const]
    }),

    searchTasks: builder.query({
      query: (term: string) => `/tasks/search?q=${term}`,
      providesTags: (result: Task[] | undefined) => ['Tasks' as const]
    }),

    getChatRooms: builder.query({
      query: () => '/chat/rooms',
      providesTags: (result: ChatRoom[] | undefined) => ['Chat' as const]
    }),

    getChatMessages: builder.query({
      query: (roomId: string) => `/chat/rooms/${roomId}/messages`,
      providesTags: (result: ChatRoom[] | undefined) => ['Chat' as const]
    }),

    sendMessage: builder.mutation({
      query: ({ roomId, content }: { roomId: string; content: string }) => ({
        url: `/chat/rooms/${roomId}/messages`,
        method: 'POST',
        body: { content }
      }),
      invalidatesTags: ['Chat' as const]
    }),

    markMessageRead: builder.mutation({
      query: ({ roomId, messageId }: { roomId: string; messageId: string }) => ({
        url: `/chat/rooms/${roomId}/messages/${messageId}/read`,
        method: 'PUT'
      }),
      invalidatesTags: ['Chat' as const]
    }),

    sendTypingStatus: builder.mutation({
      query: ({ roomId, isTyping }: { roomId: string; isTyping: boolean }) => ({
        url: `/chat/rooms/${roomId}/typing`,
        method: 'POST',
        body: { isTyping }
      })
    })
  })
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useCreateTaskMutation,
  useGetTasksQuery,
  useUpdateTaskMutation,
  useGetProfileQuery,
  useUpdateProfileMutation,
  useGetNotificationsQuery,
  useMarkNotificationReadMutation,
  useSearchTasksQuery,
  useGetChatRoomsQuery,
  useGetChatMessagesQuery,
  useSendMessageMutation,
  useMarkMessageReadMutation,
  useSendTypingStatusMutation
} = api;
