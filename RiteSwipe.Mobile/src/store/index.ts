import { configureStore, ConfigureStoreOptions, Middleware } from '@reduxjs/toolkit';
import { setupListeners } from '@reduxjs/toolkit/query';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { persistReducer, persistStore, PersistConfig } from 'redux-persist';

import { api } from '../services/api';
import authReducer from './slices/authSlice';
import taskReducer from './slices/taskSlice';

interface RootState {
  [api.reducerPath]: ReturnType<typeof api.reducer>;
  auth: ReturnType<typeof authReducer>;
  tasks: ReturnType<typeof taskReducer>;
}

const persistConfig: PersistConfig<RootState['auth']> = {
  key: 'root',
  storage: AsyncStorage,
  whitelist: ['auth']
};

const persistedAuthReducer = persistReducer(persistConfig, authReducer);

export const store = configureStore({
  reducer: {
    [api.reducerPath]: api.reducer,
    auth: persistedAuthReducer,
    tasks: taskReducer
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: {
        ignoredActions: ['persist/PERSIST']
      }
    }).concat(api.middleware as Middleware)
} as ConfigureStoreOptions);

setupListeners(store.dispatch);

export const persistor = persistStore(store);

export type { RootState };
export type AppDispatch = typeof store.dispatch;
