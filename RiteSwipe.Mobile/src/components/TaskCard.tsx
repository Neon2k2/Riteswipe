import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Image
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, shadows, borderRadius } from '@/theme';
import { Task } from '@/types';
import { formatCurrency } from '@/utils/formatters';
import UserAvatar from './UserAvatar';

interface TaskCardProps {
  task: Task;
  onPress: () => void;
}

const TaskCard: React.FC<TaskCardProps> = ({ task, onPress }) => {
  const getStatusColor = () => {
    switch (task.status) {
      case 'open':
        return colors.success;
      case 'in_progress':
        return colors.warning;
      case 'completed':
        return colors.primary;
      case 'cancelled':
        return colors.error;
      default:
        return colors.textLight;
    }
  };

  const getStatusText = () => {
    switch (task.status) {
      case 'open':
        return 'Open';
      case 'in_progress':
        return 'In Progress';
      case 'completed':
        return 'Completed';
      case 'cancelled':
        return 'Cancelled';
      default:
        return task.status;
    }
  };

  return (
    <TouchableOpacity style={styles.container} onPress={onPress}>
      {task.images?.[0] && (
        <Image source={{ uri: task.images[0] }} style={styles.image} />
      )}

      <View style={styles.content}>
        <View style={styles.header}>
          <Text style={styles.title} numberOfLines={2}>
            {task.title}
          </Text>
          <View style={[styles.status, { backgroundColor: getStatusColor() }]}>
            <Text style={styles.statusText}>{getStatusText()}</Text>
          </View>
        </View>

        <Text style={styles.description} numberOfLines={2}>
          {task.description}
        </Text>

        <View style={styles.details}>
          <View style={styles.detailItem}>
            <Icon name="currency-usd" size={16} color={colors.primary} />
            <Text style={styles.detailText}>
              {formatCurrency(task.budget.min)} - {formatCurrency(task.budget.max)}
            </Text>
          </View>

          <View style={styles.detailItem}>
            <Icon name="clock-outline" size={16} color={colors.primary} />
            <Text style={styles.detailText}>
              {task.duration.value} {task.duration.unit}
            </Text>
          </View>

          {task.location && (
            <View style={styles.detailItem}>
              <Icon name="map-marker" size={16} color={colors.primary} />
              <Text style={styles.detailText} numberOfLines={1}>
                {task.location.address}
              </Text>
            </View>
          )}
        </View>

        <View style={styles.footer}>
          <View style={styles.skills}>
            {task.requiredSkills.slice(0, 3).map((skill, index) => (
              <View key={index} style={styles.skillChip}>
                <Text style={styles.skillText}>{skill}</Text>
              </View>
            ))}
            {task.requiredSkills.length > 3 && (
              <Text style={styles.moreSkills}>
                +{task.requiredSkills.length - 3}
              </Text>
            )}
          </View>

          <View style={styles.postedBy}>
            <UserAvatar uri={task.postedBy.avatarUrl} size={24} />
            <Text style={styles.postedByText}>
              {task.postedBy.name}
            </Text>
          </View>
        </View>
      </View>
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: colors.white,
    borderRadius: borderRadius.lg,
    marginBottom: spacing.md,
    ...shadows.small
  },
  image: {
    height: 150,
    borderTopLeftRadius: borderRadius.lg,
    borderTopRightRadius: borderRadius.lg
  },
  content: {
    padding: spacing.md
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: spacing.xs
  },
  title: {
    ...typography.heading2,
    color: colors.text,
    flex: 1,
    marginRight: spacing.sm
  },
  status: {
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    borderRadius: borderRadius.round
  },
  statusText: {
    ...typography.caption,
    color: colors.white
  },
  description: {
    ...typography.body2,
    color: colors.textLight,
    marginBottom: spacing.sm
  },
  details: {
    gap: spacing.xs,
    marginBottom: spacing.md
  },
  detailItem: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs
  },
  detailText: {
    ...typography.body2,
    color: colors.text
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center'
  },
  skills: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs
  },
  skillChip: {
    backgroundColor: colors.primary + '10',
    paddingHorizontal: spacing.sm,
    paddingVertical: spacing.xs,
    borderRadius: borderRadius.round
  },
  skillText: {
    ...typography.caption,
    color: colors.primary
  },
  moreSkills: {
    ...typography.caption,
    color: colors.textLight
  },
  postedBy: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs
  },
  postedByText: {
    ...typography.caption,
    color: colors.textLight
  }
});

export default TaskCard;
