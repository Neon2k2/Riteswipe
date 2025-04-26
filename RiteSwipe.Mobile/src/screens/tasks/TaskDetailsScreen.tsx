import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  ScrollView,
  Image,
  TouchableOpacity,
  Dimensions,
  Linking
} from 'react-native';
import { useRoute, useNavigation } from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import Animated, {
  useSharedValue,
  useAnimatedStyle,
  withSpring
} from 'react-native-reanimated';

import { useGetTaskQuery, useApplyForTaskMutation } from '@/services/api';
import { colors, spacing, typography, shadows, borderRadius } from '@/theme';
import { formatCurrency, formatDistance, formatDate } from '@/utils/formatters';
import Button from '@/components/Button';
import LoadingSpinner from '@/components/LoadingSpinner';
import ErrorMessage from '@/components/ErrorMessage';
import UserAvatar from '@/components/UserAvatar';
import SkillTag from '@/components/SkillTag';
import ApplicationModal from '@/components/ApplicationModal';

const { width: SCREEN_WIDTH } = Dimensions.get('window');
const HEADER_MAX_HEIGHT = 300;
const HEADER_MIN_HEIGHT = 60;

const TaskDetailsScreen = () => {
  const route = useRoute();
  const navigation = useNavigation();
  const { taskId } = route.params;
  const { data: task, isLoading, error } = useGetTaskQuery(taskId);
  const [applyForTask] = useApplyForTaskMutation();
  const [showApplicationModal, setShowApplicationModal] = useState(false);

  const scrollY = useSharedValue(0);
  const headerHeight = useAnimatedStyle(() => {
    return {
      height: withSpring(
        Math.max(
          HEADER_MIN_HEIGHT,
          HEADER_MAX_HEIGHT - scrollY.value
        ),
        { damping: 20 }
      )
    };
  });

  const headerImageOpacity = useAnimatedStyle(() => {
    return {
      opacity: withSpring(
        Math.max(0, (HEADER_MAX_HEIGHT - scrollY.value) / HEADER_MAX_HEIGHT),
        { damping: 20 }
      )
    };
  });

  const handleApply = async (message: string) => {
    try {
      await applyForTask({
        taskId,
        application: { message }
      }).unwrap();
      setShowApplicationModal(false);
      navigation.goBack();
    } catch (error) {
      console.error('Failed to apply:', error);
    }
  };

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage message="Failed to load task details" />;
  if (!task) return null;

  return (
    <View style={styles.container}>
      <Animated.View style={[styles.header, headerHeight]}>
        <Animated.Image
          source={{ uri: task.imageUrl }}
          style={[styles.headerImage, headerImageOpacity]}
        />
        <TouchableOpacity
          style={styles.backButton}
          onPress={() => navigation.goBack()}
        >
          <Icon name="arrow-left" size={24} color={colors.white} />
        </TouchableOpacity>
      </Animated.View>

      <ScrollView
        showsVerticalScrollIndicator={false}
        onScroll={(event) => {
          scrollY.value = event.nativeEvent.contentOffset.y;
        }}
        scrollEventThrottle={16}
      >
        <View style={styles.content}>
          <View style={styles.titleSection}>
            <Text style={styles.title}>{task.title}</Text>
            <Text style={styles.budget}>{formatCurrency(task.budget)}</Text>
          </View>

          <View style={styles.infoSection}>
            <View style={styles.infoItem}>
              <Icon name="clock-outline" size={20} color={colors.primary} />
              <Text style={styles.infoText}>
                Duration: {formatDistance(task.duration)}
              </Text>
            </View>
            <View style={styles.infoItem}>
              <Icon name="map-marker-outline" size={20} color={colors.primary} />
              <Text style={styles.infoText}>{task.location}</Text>
            </View>
            <View style={styles.infoItem}>
              <Icon name="calendar-outline" size={20} color={colors.primary} />
              <Text style={styles.infoText}>
                Posted: {formatDate(task.createdAt)}
              </Text>
            </View>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Description</Text>
            <Text style={styles.description}>{task.description}</Text>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Required Skills</Text>
            <ScrollView
              horizontal
              showsHorizontalScrollIndicator={false}
              contentContainerStyle={styles.skillsContainer}
            >
              {task.skills.map((skill) => (
                <SkillTag key={skill} skill={skill} />
              ))}
            </ScrollView>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Posted By</Text>
            <TouchableOpacity
              style={styles.posterContainer}
              onPress={() => navigation.navigate('UserProfile', { userId: task.userId })}
            >
              <UserAvatar
                uri={task.user.avatarUrl}
                size={50}
                style={styles.posterAvatar}
              />
              <View style={styles.posterInfo}>
                <Text style={styles.posterName}>{task.user.name}</Text>
                <Text style={styles.posterRating}>
                  <Icon name="star" size={16} color={colors.warning} />
                  {` ${task.user.rating.toFixed(1)} (${task.user.reviewCount} reviews)`}
                </Text>
              </View>
              <Icon name="chevron-right" size={24} color={colors.textLight} />
            </TouchableOpacity>
          </View>

          <View style={styles.section}>
            <Text style={styles.sectionTitle}>Location</Text>
            <TouchableOpacity
              style={styles.mapPreview}
              onPress={() => Linking.openURL(`https://maps.google.com/?q=${task.location}`)}
            >
              <Image
                source={{ uri: `https://maps.googleapis.com/maps/api/staticmap?center=${task.location}&zoom=13&size=${SCREEN_WIDTH - 32}x200&key=YOUR_API_KEY` }}
                style={styles.mapImage}
              />
              <View style={styles.mapOverlay}>
                <Icon name="map-marker" size={24} color={colors.primary} />
                <Text style={styles.mapText}>View on Maps</Text>
              </View>
            </TouchableOpacity>
          </View>
        </View>
      </ScrollView>

      <View style={styles.footer}>
        <Button
          title="Apply for Task"
          icon="handshake"
          onPress={() => setShowApplicationModal(true)}
          style={styles.applyButton}
        />
      </View>

      <ApplicationModal
        visible={showApplicationModal}
        onClose={() => setShowApplicationModal(false)}
        onSubmit={handleApply}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background
  },
  header: {
    width: '100%',
    overflow: 'hidden'
  },
  headerImage: {
    width: '100%',
    height: '100%'
  },
  backButton: {
    position: 'absolute',
    top: spacing.lg,
    left: spacing.md,
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: colors.overlay,
    justifyContent: 'center',
    alignItems: 'center'
  },
  content: {
    padding: spacing.lg
  },
  titleSection: {
    marginBottom: spacing.lg
  },
  title: {
    ...typography.heading1,
    color: colors.text,
    marginBottom: spacing.xs
  },
  budget: {
    ...typography.heading2,
    color: colors.primary
  },
  infoSection: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    backgroundColor: colors.white,
    padding: spacing.md,
    borderRadius: borderRadius.lg,
    marginBottom: spacing.lg,
    ...shadows.small
  },
  infoItem: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs
  },
  infoText: {
    ...typography.body2,
    color: colors.text
  },
  section: {
    marginBottom: spacing.xl
  },
  sectionTitle: {
    ...typography.heading3,
    color: colors.text,
    marginBottom: spacing.md
  },
  description: {
    ...typography.body1,
    color: colors.text,
    lineHeight: 24
  },
  skillsContainer: {
    gap: spacing.sm,
    paddingVertical: spacing.xs
  },
  posterContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colors.white,
    padding: spacing.md,
    borderRadius: borderRadius.lg,
    ...shadows.small
  },
  posterAvatar: {
    marginRight: spacing.md
  },
  posterInfo: {
    flex: 1
  },
  posterName: {
    ...typography.body1,
    color: colors.text,
    marginBottom: spacing.xs
  },
  posterRating: {
    ...typography.body2,
    color: colors.textLight
  },
  mapPreview: {
    borderRadius: borderRadius.lg,
    overflow: 'hidden',
    height: 200,
    ...shadows.small
  },
  mapImage: {
    width: '100%',
    height: '100%'
  },
  mapOverlay: {
    position: 'absolute',
    bottom: 0,
    left: 0,
    right: 0,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: colors.overlay,
    padding: spacing.sm,
    gap: spacing.xs
  },
  mapText: {
    ...typography.body2,
    color: colors.white
  },
  footer: {
    backgroundColor: colors.white,
    padding: spacing.md,
    borderTopWidth: 1,
    borderTopColor: colors.border
  },
  applyButton: {
    height: 50
  }
});

export default TaskDetailsScreen;
