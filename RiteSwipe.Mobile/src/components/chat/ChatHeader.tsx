import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Platform
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useGetUserProfileQuery } from '@/services/api';
import { colors, spacing, typography, shadows } from '@/theme';
import UserAvatar from '@/components/UserAvatar';

interface ChatHeaderProps {
  recipientId: string;
  onBack: () => void;
}

const ChatHeader: React.FC<ChatHeaderProps> = ({ recipientId, onBack }) => {
  const { data: recipient } = useGetUserProfileQuery(recipientId);

  return (
    <View style={styles.container}>
      <TouchableOpacity
        onPress={onBack}
        style={styles.backButton}
        hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
      >
        <Icon name="arrow-left" size={24} color={colors.text} />
      </TouchableOpacity>

      <View style={styles.userInfo}>
        <UserAvatar
          uri={recipient?.avatarUrl}
          size={40}
          style={styles.avatar}
        />
        <View>
          <Text style={styles.name}>{recipient?.name || 'Loading...'}</Text>
          {recipient?.isOnline && (
            <View style={styles.onlineStatus}>
              <View style={styles.onlineDot} />
              <Text style={styles.onlineText}>Online</Text>
            </View>
          )}
        </View>
      </View>

      <View style={styles.actions}>
        <TouchableOpacity
          style={styles.actionButton}
          hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
        >
          <Icon name="video" size={24} color={colors.primary} />
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.actionButton}
          hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
        >
          <Icon name="phone" size={24} color={colors.primary} />
        </TouchableOpacity>
        <TouchableOpacity
          style={styles.actionButton}
          hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
        >
          <Icon name="dots-vertical" size={24} color={colors.text} />
        </TouchableOpacity>
      </View>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: spacing.md,
    paddingTop: Platform.OS === 'ios' ? 44 : spacing.md,
    paddingBottom: spacing.md,
    backgroundColor: colors.white,
    borderBottomWidth: 1,
    borderBottomColor: colors.border,
    ...shadows.small
  },
  backButton: {
    marginRight: spacing.md
  },
  userInfo: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center'
  },
  avatar: {
    marginRight: spacing.sm
  },
  name: {
    ...typography.body1,
    color: colors.text
  },
  onlineStatus: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  onlineDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: colors.success,
    marginRight: spacing.xs
  },
  onlineText: {
    ...typography.caption,
    color: colors.textLight
  },
  actions: {
    flexDirection: 'row',
    alignItems: 'center'
  },
  actionButton: {
    padding: spacing.xs,
    marginLeft: spacing.sm
  }
});

export default ChatHeader;
