import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { User, Profile } from '../../types';

interface UserState {
  profile: Profile | null;
  skills: string[];
  ratings: number[];
  completedTasks: number;
  isLoading: boolean;
  error: string | null;
}

const initialState: UserState = {
  profile: null,
  skills: [],
  ratings: [],
  completedTasks: 0,
  isLoading: false,
  error: null
};

export const userSlice = createSlice({
  name: 'user',
  initialState,
  reducers: {
    setProfile: (state, action: PayloadAction<Profile>) => {
      state.profile = action.payload;
    },
    setSkills: (state, action: PayloadAction<string[]>) => {
      state.skills = action.payload;
    },
    setRatings: (state, action: PayloadAction<number[]>) => {
      state.ratings = action.payload;
    },
    setCompletedTasks: (state, action: PayloadAction<number>) => {
      state.completedTasks = action.payload;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.isLoading = action.payload;
    },
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
    updateProfile: (state, action: PayloadAction<Partial<Profile>>) => {
      if (state.profile) {
        state.profile = { ...state.profile, ...action.payload };
      }
    },
    addSkill: (state, action: PayloadAction<string>) => {
      if (!state.skills.includes(action.payload)) {
        state.skills.push(action.payload);
      }
    },
    removeSkill: (state, action: PayloadAction<string>) => {
      state.skills = state.skills.filter(skill => skill !== action.payload);
    },
    addRating: (state, action: PayloadAction<number>) => {
      state.ratings.push(action.payload);
    }
  }
});

export const {
  setProfile,
  setSkills,
  setRatings,
  setCompletedTasks,
  setLoading,
  setError,
  updateProfile,
  addSkill,
  removeSkill,
  addRating
} = userSlice.actions;

export default userSlice.reducer;
