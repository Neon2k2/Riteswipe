import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  FlatList,
  TouchableOpacity,
  RefreshControl,
  Animated
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';

import {
  useGetNotificationsQuery,
  useMarkNotificationReadMutation
} from '@/services/api';
import { colors, spacing, typography, shadows, borderRadius } from '@/theme';
import { formatTimeAgo } from '@/utils/formatters';
import UserAvatar from '@/components/UserAvatar';
import LoadingSpinner from '@/components/LoadingSpinner';
import ErrorMessage from '@/components/ErrorMessage';

const getNotificationIcon = (type: string) => {
  switch (type) {
    case 'TASK_APPLICATION':
      return { name: 'hand-wave', color: colors.primary };
    case 'TASK_ACCEPTED':
      return { name: 'check-circle', color: colors.success };
    case 'TASK_REJECTED':
      return { name: 'close-circle', color: colors.error };
    case 'PAYMENT_RECEIVED':
      return { name: 'cash', color: colors.success };
    case 'NEW_MESSAGE':
      return { name: 'message', color: colors.info };
    case 'REVIEW_RECEIVED':
      return { name: 'star', color: colors.warning };
    default:
      return { name: 'bell', color: colors.primary };
  }
};

const NotificationItem = React.memo(({ item, onPress, onMarkRead }: any) => {
  const fadeAnim = React.useRef(new Animated.Value(1)).current;
  const icon = getNotificationIcon(item.type);

  const handleMarkRead = () => {
    Animated.timing(fadeAnim, {
      toValue: 0.5,
      duration: 300,
      useNativeDriver: true
    }).start(() => {
      onMarkRead(item.id);
    });
  };

  return (
    <Animated.View style={[styles.notificationItem, { opacity: fadeAnim }]}>
      <TouchableOpacity
        style={[
          styles.notificationContent,
          !item.read && styles.unreadNotification
        ]}
        onPress={() => onPress(item)}
      >
        <View style={styles.avatarContainer}>
          <UserAvatar uri={item.sender?.avatarUrl} size={50} />
          <View
            style={[styles.iconBadge, { backgroundColor: icon.color }]}
          >
            <Icon name={icon.name} size={12} color={colors.white} />
          </View>
        </View>

        <View style={styles.textContainer}>
          <Text style={styles.title}>{item.title}</Text>
          <Text style={styles.message} numberOfLines={2}>
            {item.message}
          </Text>
          <Text style={styles.time}>{formatTimeAgo(item.createdAt)}</Text>
        </View>

        {!item.read && (
          <TouchableOpacity
            style={styles.markReadButton}
            onPress={handleMarkRead}
          >
            <Icon name="check" size={20} color={colors.primary} />
          </TouchableOpacity>
        )}
      </TouchableOpacity>
    </Animated.View>
  );
});

const NotificationsScreen = () => {
  const navigation = useNavigation();
  const { data: notifications, isLoading, error, refetch } = useGetNotificationsQuery();
  const [markNotificationRead] = useMarkNotificationReadMutation();

  const handleNotificationPress = (notification: any) => {
    switch (notification.type) {
      case 'TASK_APPLICATION':
      case 'TASK_ACCEPTED':
      case 'TASK_REJECTED':
        navigation.navigate('TaskDetails', { taskId: notification.taskId });
        break;
      case 'NEW_MESSAGE':
        navigation.navigate('Chat', { userId: notification.sender.id });
        break;
      case 'REVIEW_RECEIVED':
        navigation.navigate('Reviews', { userId: notification.targetId });
        break;
      default:
        break;
    }
  };

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage message="Failed to load notifications" />;

  return (
    <View style={styles.container}>
      <FlatList
        data={notifications}
        renderItem={({ item }) => (
          <NotificationItem
            item={item}
            onPress={handleNotificationPress}
            onMarkRead={markNotificationRead}
          />
        )}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.listContent}
        refreshControl={
          <RefreshControl refreshing={isLoading} onRefresh={refetch} />
        }
        ListEmptyComponent={
          <View style={styles.emptyContainer}>
            <Icon
              name="bell-off-outline"
              size={64}
              color={colors.textLight}
            />
            <Text style={styles.emptyText}>No notifications yet</Text>
          </View>
        }
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background
  },
  listContent: {
    padding: spacing.md,
    gap: spacing.md
  },
  notificationItem: {
    backgroundColor: colors.white,
    borderRadius: borderRadius.lg,
    ...shadows.small
  },
  notificationContent: {
    flexDirection: 'row',
    padding: spacing.md,
    alignItems: 'center'
  },
  unreadNotification: {
    backgroundColor: colors.primary + '05',
    borderLeftWidth: 4,
    borderLeftColor: colors.primary
  },
  avatarContainer: {
    position: 'relative',
    marginRight: spacing.md
  },
  iconBadge: {
    position: 'absolute',
    right: -4,
    bottom: -4,
    width: 24,
    height: 24,
    borderRadius: 12,
    backgroundColor: colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 2,
    borderColor: colors.white
  },
  textContainer: {
    flex: 1,
    marginRight: spacing.md
  },
  title: {
    ...typography.body1,
    color: colors.text,
    marginBottom: spacing.xs
  },
  message: {
    ...typography.body2,
    color: colors.textLight,
    marginBottom: spacing.xs
  },
  time: {
    ...typography.caption,
    color: colors.textLight
  },
  markReadButton: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: colors.primary + '10',
    justifyContent: 'center',
    alignItems: 'center'
  },
  emptyContainer: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    paddingVertical: spacing.xl * 2
  },
  emptyText: {
    ...typography.body1,
    color: colors.textLight,
    marginTop: spacing.md
  }
});

export default NotificationsScreen;
