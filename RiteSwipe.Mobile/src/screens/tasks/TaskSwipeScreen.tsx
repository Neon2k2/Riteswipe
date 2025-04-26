import React, { useRef, useState } from 'react';
import {
  View,
  StyleSheet,
  Animated,
  PanResponder,
  Dimensions,
  Text,
  Image
} from 'react-native';
import { useGetTasksQuery } from '@/services/api';
import { Task } from '@/types';
import { colors, spacing, typography, shadows } from '@/theme';
import { formatCurrency, formatDistance } from '@/utils/formatters';
import Button from '@/components/Button';
import LoadingSpinner from '@/components/LoadingSpinner';
import ErrorMessage from '@/components/ErrorMessage';

const SCREEN_WIDTH = Dimensions.get('window').width;
const SWIPE_THRESHOLD = 0.25 * SCREEN_WIDTH;
const SWIPE_OUT_DURATION = 250;

const TaskSwipeScreen = () => {
  const [taskIndex, setTaskIndex] = useState(0);
  const { data: tasks, isLoading, error } = useGetTasksQuery({});
  const position = useRef(new Animated.ValueXY()).current;

  const panResponder = useRef(
    PanResponder.create({
      onStartShouldSetPanResponder: () => true,
      onPanResponderMove: (event, gesture) => {
        position.setValue({ x: gesture.dx, y: gesture.dy });
      },
      onPanResponderRelease: (event, gesture) => {
        if (gesture.dx > SWIPE_THRESHOLD) {
          forceSwipe('right');
        } else if (gesture.dx < -SWIPE_THRESHOLD) {
          forceSwipe('left');
        } else {
          resetPosition();
        }
      }
    })
  ).current;

  const forceSwipe = (direction: 'right' | 'left') => {
    const x = direction === 'right' ? SCREEN_WIDTH : -SCREEN_WIDTH;
    Animated.timing(position, {
      toValue: { x, y: 0 },
      duration: SWIPE_OUT_DURATION,
      useNativeDriver: false
    }).start(() => onSwipeComplete(direction));
  };

  const onSwipeComplete = (direction: 'right' | 'left') => {
    const item = tasks?.[taskIndex];
    direction === 'right' ? console.log('Liked', item?.id) : console.log('Nope', item?.id);
    position.setValue({ x: 0, y: 0 });
    setTaskIndex(prevIndex => prevIndex + 1);
  };

  const resetPosition = () => {
    Animated.spring(position, {
      toValue: { x: 0, y: 0 },
      useNativeDriver: false
    }).start();
  };

  const getCardStyle = () => {
    const rotate = position.x.interpolate({
      inputRange: [-SCREEN_WIDTH * 1.5, 0, SCREEN_WIDTH * 1.5],
      outputRange: ['-120deg', '0deg', '120deg']
    });

    return {
      ...position.getLayout(),
      transform: [{ rotate }]
    };
  };

  if (isLoading) return <LoadingSpinner />;
  if (error) return <ErrorMessage message="Failed to load tasks" />;
  if (!tasks?.length) return <Text style={styles.emptyText}>No more tasks</Text>;

  const renderCard = (task: Task) => {
    return (
      <View style={styles.card}>
        <Image source={{ uri: task.imageUrl }} style={styles.image} />
        <View style={styles.content}>
          <Text style={styles.title}>{task.title}</Text>
          <Text style={styles.budget}>{formatCurrency(task.budget)}</Text>
          <Text style={styles.description}>{task.description}</Text>
          <View style={styles.footer}>
            <Text style={styles.duration}>
              Duration: {formatDistance(task.duration)}
            </Text>
            <Text style={styles.location}>{task.location}</Text>
          </View>
        </View>
      </View>
    );
  };

  return (
    <View style={styles.container}>
      <View style={styles.actionsContainer}>
        <Button
          onPress={() => forceSwipe('left')}
          style={styles.actionButton}
          icon="close"
          color={colors.error}
        />
        <Button
          onPress={() => forceSwipe('right')}
          style={styles.actionButton}
          icon="check"
          color={colors.success}
        />
      </View>

      {tasks.map((task, index) => {
        if (index < taskIndex) return null;
        if (index === taskIndex) {
          return (
            <Animated.View
              key={task.id}
              style={[getCardStyle(), styles.cardContainer]}
              {...panResponder.panHandlers}
            >
              {renderCard(task)}
            </Animated.View>
          );
        }
        return (
          <View
            key={task.id}
            style={[styles.cardContainer, { top: 10 * (index - taskIndex) }]}
          >
            {renderCard(task)}
          </View>
        );
      }).reverse()}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background,
    justifyContent: 'center',
    alignItems: 'center'
  },
  cardContainer: {
    position: 'absolute',
    width: SCREEN_WIDTH * 0.9,
    height: '70%',
    ...shadows.large
  },
  card: {
    flex: 1,
    borderRadius: 20,
    backgroundColor: colors.white,
    overflow: 'hidden'
  },
  image: {
    width: '100%',
    height: '40%'
  },
  content: {
    flex: 1,
    padding: spacing.md
  },
  title: {
    ...typography.heading2,
    marginBottom: spacing.xs
  },
  budget: {
    ...typography.heading3,
    color: colors.primary,
    marginBottom: spacing.sm
  },
  description: {
    ...typography.body1,
    color: colors.text,
    marginBottom: spacing.md
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginTop: 'auto'
  },
  duration: {
    ...typography.body2,
    color: colors.textLight
  },
  location: {
    ...typography.body2,
    color: colors.textLight
  },
  actionsContainer: {
    position: 'absolute',
    bottom: spacing.xl,
    flexDirection: 'row',
    justifyContent: 'space-around',
    width: '100%',
    paddingHorizontal: spacing.xl,
    zIndex: 1
  },
  actionButton: {
    width: 60,
    height: 60,
    borderRadius: 30,
    justifyContent: 'center',
    alignItems: 'center',
    ...shadows.medium
  },
  emptyText: {
    ...typography.heading3,
    color: colors.textLight,
    textAlign: 'center'
  }
});

export default TaskSwipeScreen;
