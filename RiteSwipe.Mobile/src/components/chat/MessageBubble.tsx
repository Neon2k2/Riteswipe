import React from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  Animated,
  Easing
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, borderRadius } from '@/theme';
import { formatTime } from '@/utils/formatters';

interface MessageBubbleProps {
  message: {
    content: string;
    timestamp: string;
    status?: 'sent' | 'delivered' | 'read';
  };
  isOwnMessage: boolean;
}

const MessageBubble: React.FC<MessageBubbleProps> = ({
  message,
  isOwnMessage
}) => {
  const scaleAnim = React.useRef(new Animated.Value(0)).current;

  React.useEffect(() => {
    Animated.spring(scaleAnim, {
      toValue: 1,
      useNativeDriver: true,
      tension: 100,
      friction: 8
    }).start();
  }, []);

  const getStatusIcon = () => {
    switch (message.status) {
      case 'sent':
        return 'check';
      case 'delivered':
        return 'check-all';
      case 'read':
        return 'check-all';
      default:
        return 'clock-outline';
    }
  };

  const getStatusColor = () => {
    return message.status === 'read' ? colors.primary : colors.textLight;
  };

  return (
    <Animated.View
      style={[
        styles.container,
        isOwnMessage ? styles.ownContainer : styles.otherContainer,
        { transform: [{ scale: scaleAnim }] }
      ]}
    >
      <View
        style={[
          styles.bubble,
          isOwnMessage ? styles.ownBubble : styles.otherBubble
        ]}
      >
        <Text
          style={[
            styles.message,
            isOwnMessage ? styles.ownMessage : styles.otherMessage
          ]}
        >
          {message.content}
        </Text>
        
        <View style={styles.footer}>
          <Text
            style={[
              styles.timestamp,
              isOwnMessage ? styles.ownTimestamp : styles.otherTimestamp
            ]}
          >
            {formatTime(message.timestamp)}
          </Text>
          
          {isOwnMessage && (
            <Icon
              name={getStatusIcon()}
              size={16}
              color={getStatusColor()}
              style={styles.statusIcon}
            />
          )}
        </View>
      </View>

      <TouchableOpacity
        style={styles.reactionButton}
        hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
      >
        <Icon
          name="emoticon-outline"
          size={16}
          color={colors.textLight}
        />
      </TouchableOpacity>
    </Animated.View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    marginVertical: spacing.xs,
    maxWidth: '80%'
  },
  ownContainer: {
    alignSelf: 'flex-end'
  },
  otherContainer: {
    alignSelf: 'flex-start'
  },
  bubble: {
    padding: spacing.sm,
    borderRadius: borderRadius.lg,
    maxWidth: '100%'
  },
  ownBubble: {
    backgroundColor: colors.primary,
    borderBottomRightRadius: spacing.xs,
    marginLeft: spacing.md
  },
  otherBubble: {
    backgroundColor: colors.white,
    borderBottomLeftRadius: spacing.xs,
    marginRight: spacing.md
  },
  message: {
    ...typography.body1,
    marginBottom: spacing.xs
  },
  ownMessage: {
    color: colors.white
  },
  otherMessage: {
    color: colors.text
  },
  footer: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'flex-end'
  },
  timestamp: {
    ...typography.caption
  },
  ownTimestamp: {
    color: colors.white + '99'
  },
  otherTimestamp: {
    color: colors.textLight
  },
  statusIcon: {
    marginLeft: spacing.xs
  },
  reactionButton: {
    width: 28,
    height: 28,
    borderRadius: 14,
    backgroundColor: colors.white,
    justifyContent: 'center',
    alignItems: 'center',
    marginHorizontal: spacing.xs,
    shadowColor: colors.black,
    shadowOffset: {
      width: 0,
      height: 2
    },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2
  }
});

export default MessageBubble;
