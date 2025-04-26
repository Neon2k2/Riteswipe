import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  TouchableOpacity,
  RefreshControl
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useSelector } from 'react-redux';

import { useGetUserProfileQuery } from '@/services/api';
import { RootState } from '@/store';
import { colors, spacing, typography, shadows, borderRadius } from '@/theme';
import UserAvatar from '@/components/UserAvatar';
import StatCard from '@/components/StatCard';
import TaskCard from '@/components/TaskCard';
import LoadingSpinner from '@/components/LoadingSpinner';
import ErrorMessage from '@/components/ErrorMessage';
import Button from '@/components/Button';

const ProfileScreen = () => {
  const navigation = useNavigation();
  const { user } = useSelector((state: RootState) => state.auth);
  const { data: profile, isLoading, error, refetch } = useGetUserProfileQuery();

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage message="Failed to load profile" />;
  if (!profile) return null;

  const stats = [
    {
      icon: 'check-circle',
      value: profile.completedTasks,
      label: 'Tasks Completed'
    },
    {
      icon: 'star',
      value: profile.rating.toFixed(1),
      label: 'Rating'
    },
    {
      icon: 'currency-usd',
      value: profile.totalEarnings,
      label: 'Total Earnings',
      isCurrency: true
    }
  ];

  return (
    <ScrollView
      style={styles.container}
      refreshControl={
        <RefreshControl refreshing={isLoading} onRefresh={refetch} />
      }
    >
      <View style={styles.header}>
        <View style={styles.headerContent}>
          <UserAvatar uri={profile.avatarUrl} size={100} style={styles.avatar} />
          <Text style={styles.name}>{profile.name}</Text>
          <Text style={styles.tagline}>{profile.tagline}</Text>
          
          <View style={styles.locationContainer}>
            <Icon name="map-marker" size={16} color={colors.primary} />
            <Text style={styles.location}>{profile.location}</Text>
          </View>

          <Button
            title="Edit Profile"
            icon="pencil"
            variant="outline"
            onPress={() => navigation.navigate('EditProfile')}
            style={styles.editButton}
          />
        </View>
      </View>

      <View style={styles.statsContainer}>
        {stats.map((stat, index) => (
          <StatCard
            key={index}
            icon={stat.icon}
            value={stat.value}
            label={stat.label}
            isCurrency={stat.isCurrency}
          />
        ))}
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>About Me</Text>
        <Text style={styles.bio}>{profile.bio}</Text>
      </View>

      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Skills</Text>
        <View style={styles.skillsContainer}>
          {profile.skills.map((skill) => (
            <View key={skill} style={styles.skillChip}>
              <Text style={styles.skillText}>{skill}</Text>
            </View>
          ))}
        </View>
      </View>

      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Recent Reviews</Text>
          <TouchableOpacity
            onPress={() => navigation.navigate('Reviews', { userId: profile.id })}
          >
            <Text style={styles.seeAllText}>See All</Text>
          </TouchableOpacity>
        </View>
        {profile.recentReviews.map((review) => (
          <View key={review.id} style={styles.reviewCard}>
            <View style={styles.reviewHeader}>
              <UserAvatar uri={review.reviewer.avatarUrl} size={40} />
              <View style={styles.reviewerInfo}>
                <Text style={styles.reviewerName}>{review.reviewer.name}</Text>
                <View style={styles.ratingContainer}>
                  {Array.from({ length: 5 }).map((_, index) => (
                    <Icon
                      key={index}
                      name={index < review.rating ? 'star' : 'star-outline'}
                      size={16}
                      color={colors.warning}
                    />
                  ))}
                </View>
              </View>
              <Text style={styles.reviewDate}>{review.date}</Text>
            </View>
            <Text style={styles.reviewText}>{review.comment}</Text>
          </View>
        ))}
      </View>

      <View style={styles.section}>
        <View style={styles.sectionHeader}>
          <Text style={styles.sectionTitle}>Active Tasks</Text>
          <TouchableOpacity
            onPress={() => navigation.navigate('MyTasks')}
          >
            <Text style={styles.seeAllText}>See All</Text>
          </TouchableOpacity>
        </View>
        {profile.activeTasks.map((task) => (
          <TaskCard
            key={task.id}
            task={task}
            onPress={() => navigation.navigate('TaskDetails', { taskId: task.id })}
          />
        ))}
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background
  },
  header: {
    backgroundColor: colors.primary,
    paddingTop: spacing.xl * 2,
    borderBottomLeftRadius: borderRadius.xl,
    borderBottomRightRadius: borderRadius.xl
  },
  headerContent: {
    alignItems: 'center',
    paddingBottom: spacing.xl
  },
  avatar: {
    borderWidth: 4,
    borderColor: colors.white,
    marginBottom: spacing.md
  },
  name: {
    ...typography.heading1,
    color: colors.white,
    marginBottom: spacing.xs
  },
  tagline: {
    ...typography.body1,
    color: colors.white,
    opacity: 0.9,
    marginBottom: spacing.sm
  },
  locationContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colors.white,
    paddingVertical: spacing.xs,
    paddingHorizontal: spacing.sm,
    borderRadius: borderRadius.round,
    marginBottom: spacing.md
  },
  location: {
    ...typography.body2,
    color: colors.text,
    marginLeft: spacing.xs
  },
  editButton: {
    backgroundColor: colors.white,
    paddingHorizontal: spacing.xl
  },
  statsContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    padding: spacing.lg,
    marginTop: -spacing.xl,
    marginHorizontal: spacing.md
  },
  section: {
    padding: spacing.lg,
    marginBottom: spacing.md
  },
  sectionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.md
  },
  sectionTitle: {
    ...typography.heading2,
    color: colors.text
  },
  seeAllText: {
    ...typography.body2,
    color: colors.primary
  },
  bio: {
    ...typography.body1,
    color: colors.text,
    lineHeight: 24
  },
  skillsContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm,
    marginTop: spacing.sm
  },
  skillChip: {
    backgroundColor: colors.primary,
    paddingVertical: spacing.xs,
    paddingHorizontal: spacing.sm,
    borderRadius: borderRadius.round
  },
  skillText: {
    ...typography.body2,
    color: colors.white
  },
  reviewCard: {
    backgroundColor: colors.white,
    padding: spacing.md,
    borderRadius: borderRadius.lg,
    marginBottom: spacing.md,
    ...shadows.small
  },
  reviewHeader: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: spacing.sm
  },
  reviewerInfo: {
    flex: 1,
    marginLeft: spacing.sm
  },
  reviewerName: {
    ...typography.body1,
    color: colors.text
  },
  ratingContainer: {
    flexDirection: 'row',
    gap: spacing.xs
  },
  reviewDate: {
    ...typography.caption,
    color: colors.textLight
  },
  reviewText: {
    ...typography.body2,
    color: colors.text
  }
});

export default ProfileScreen;
