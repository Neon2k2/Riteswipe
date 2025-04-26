export interface User {
  id: string;
  name: string;
  email: string;
  avatar?: string;
  bio?: string;
  skills: string[];
  rating: number;
  tasksCompleted: number;
  tasksPosted: number;
  isOnline?: boolean;
  createdAt: string;
}

export interface Task {
  id: string;
  title: string;
  description: string;
  budget: {
    min: number;
    max: number;
  };
  location?: {
    latitude: number;
    longitude: number;
    address: string;
  };
  duration: {
    value: number;
    unit: 'hours' | 'days' | 'weeks' | 'months';
  };
  requiredSkills: string[];
  images?: string[];
  status: 'open' | 'in_progress' | 'completed' | 'cancelled';
  createdAt: string;
  updatedAt: string;
  postedBy: User;
  assignedTo?: User;
  applications: TaskApplication[];
}

export interface TaskApplication {
  id: string;
  task: Task;
  applicant: User;
  coverLetter: string;
  proposedPrice: number;
  status: 'pending' | 'accepted' | 'rejected';
  createdAt: string;
}

export interface Review {
  id: string;
  task: Task;
  reviewer: User;
  reviewee: User;
  rating: number;
  comment: string;
  createdAt: string;
}

export interface Notification {
  id: string;
  type: 'TASK_APPLICATION' | 'TASK_ACCEPTED' | 'TASK_REJECTED' | 'PAYMENT_RECEIVED' | 'NEW_MESSAGE' | 'REVIEW_RECEIVED';
  title: string;
  message: string;
  read: boolean;
  createdAt: string;
  sender?: User;
  taskId?: string;
  targetId?: string;
}

export interface Message {
  id: string;
  content: string;
  senderId: string;
  recipientId: string;
  timestamp: string;
  status?: 'sent' | 'delivered' | 'read';
}

export interface ChatMessage {
  id: string;
  chatRoom: ChatRoom;
  sender: User;
  content: string;
  read: boolean;
  createdAt: string;
}

export interface ChatRoom {
  id: string;
  participants: User[];
  lastMessage?: ChatMessage;
  unreadCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Profile {
  id: string;
  user: User;
  bio: string;
  avatar: string;
  skills: string[];
  hourlyRate: number;
  availability: string;
  location: Location;
  socialLinks: SocialLinks;
  preferences: UserPreferences;
}

export interface Location {
  latitude: number;
  longitude: number;
  address: string;
  city: string;
  state: string;
  country: string;
  postalCode: string;
}

export interface SocialLinks {
  linkedin?: string;
  github?: string;
  twitter?: string;
  portfolio?: string;
}

export interface UserPreferences {
  notifications: NotificationPreferences;
  privacy: PrivacySettings;
  theme: 'light' | 'dark' | 'system';
  language: string;
}

export interface NotificationPreferences {
  email: boolean;
  push: boolean;
  taskUpdates: boolean;
  messages: boolean;
  marketing: boolean;
}

export interface PrivacySettings {
  profileVisibility: 'public' | 'private' | 'connections';
  showEmail: boolean;
  showPhone: boolean;
  showLocation: boolean;
}

export enum TaskStatus {
  Open = 'open',
  InProgress = 'in_progress',
  Completed = 'completed',
  Cancelled = 'cancelled'
}

export enum ApplicationStatus {
  Pending = 'pending',
  Accepted = 'accepted',
  Rejected = 'rejected'
}

export enum NotificationType {
  TaskApplication = 'task_application',
  ApplicationAccepted = 'application_accepted',
  ApplicationRejected = 'application_rejected',
  TaskCompleted = 'task_completed',
  NewMessage = 'new_message',
  NewReview = 'new_review'
}
