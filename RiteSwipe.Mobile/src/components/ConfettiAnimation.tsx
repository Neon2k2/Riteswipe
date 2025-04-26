import React, { useEffect } from 'react';
import { StyleSheet, View, Dimensions } from 'react-native';
import Animated, {
  useAnimatedStyle,
  useSharedValue,
  withDelay,
  withSequence,
  withTiming,
  Easing
} from 'react-native-reanimated';
import { colors } from '@/theme';

const { width: SCREEN_WIDTH, height: SCREEN_HEIGHT } = Dimensions.get('window');
const CONFETTI_COUNT = 50;
const CONFETTI_COLORS = [
  colors.primary,
  colors.secondary,
  colors.success,
  colors.warning,
  colors.info
];

interface ConfettiProps {
  index: number;
}

const Confetti: React.FC<ConfettiProps> = ({ index }) => {
  const translateY = useSharedValue(-50);
  const translateX = useSharedValue(0);
  const rotate = useSharedValue(0);
  const opacity = useSharedValue(1);

  useEffect(() => {
    const delay = index * 50;
    const startX = Math.random() * SCREEN_WIDTH;
    const endX = startX + (Math.random() - 0.5) * 200;
    const rotations = 5 + Math.random() * 5;

    translateY.value = withDelay(
      delay,
      withTiming(SCREEN_HEIGHT + 50, {
        duration: 2000,
        easing: Easing.bezier(0.25, 0.1, 0.25, 1)
      })
    );

    translateX.value = withDelay(
      delay,
      withSequence(
        withTiming(endX, {
          duration: 2000,
          easing: Easing.bezier(0.25, 0.1, 0.25, 1)
        })
      )
    );

    rotate.value = withDelay(
      delay,
      withTiming(360 * rotations, {
        duration: 2000,
        easing: Easing.linear
      })
    );

    opacity.value = withDelay(
      delay + 1500,
      withTiming(0, {
        duration: 500,
        easing: Easing.linear
      })
    );
  }, []);

  const animatedStyle = useAnimatedStyle(() => {
    return {
      transform: [
        { translateY: translateY.value },
        { translateX: translateX.value },
        { rotate: `${rotate.value}deg` }
      ],
      opacity: opacity.value
    };
  });

  const color = CONFETTI_COLORS[index % CONFETTI_COLORS.length];
  const size = 8 + Math.random() * 8;
  const isRectangle = Math.random() > 0.5;

  return (
    <Animated.View
      style={[
        styles.confetti,
        animatedStyle,
        {
          width: isRectangle ? size : size / 2,
          height: size,
          backgroundColor: color,
          borderRadius: isRectangle ? 2 : size / 2
        }
      ]}
    />
  );
};

const ConfettiAnimation = () => {
  return (
    <View style={styles.container} pointerEvents="none">
      {Array.from({ length: CONFETTI_COUNT }).map((_, index) => (
        <Confetti key={index} index={index} />
      ))}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    ...StyleSheet.absoluteFillObject,
    zIndex: 1000
  },
  confetti: {
    position: 'absolute'
  }
});

export default ConfettiAnimation;
