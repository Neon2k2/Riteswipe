import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import Slider from '@react-native-community/slider';
import { colors, spacing, typography } from '@/theme';
import { formatCurrency } from '@/utils/formatters';

interface PriceRangeSliderProps {
  value: number;
  onValueChange: (value: number) => void;
  error?: string;
}

const PriceRangeSlider: React.FC<PriceRangeSliderProps> = ({
  value,
  onValueChange,
  error
}) => {
  const minValue = 10;
  const maxValue = 1000;
  const step = 10;

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.label}>Budget</Text>
        <Text style={styles.value}>{formatCurrency(value)}</Text>
      </View>

      <View style={styles.sliderContainer}>
        <Slider
          style={styles.slider}
          minimumValue={minValue}
          maximumValue={maxValue}
          step={step}
          value={value}
          onValueChange={onValueChange}
          minimumTrackTintColor={colors.primary}
          maximumTrackTintColor={colors.border}
          thumbTintColor={colors.primary}
        />
        <View style={styles.rangeLabels}>
          <Text style={styles.rangeText}>{formatCurrency(minValue)}</Text>
          <Text style={styles.rangeText}>{formatCurrency(maxValue)}</Text>
        </View>
      </View>

      {error && <Text style={styles.error}>{error}</Text>}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginBottom: spacing.md
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.sm
  },
  label: {
    ...typography.body1,
    color: colors.text
  },
  value: {
    ...typography.heading3,
    color: colors.primary
  },
  sliderContainer: {
    backgroundColor: colors.white,
    padding: spacing.md,
    borderRadius: 12,
    borderWidth: 1,
    borderColor: colors.border
  },
  slider: {
    height: 40
  },
  rangeLabels: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginTop: -spacing.sm
  },
  rangeText: {
    ...typography.caption,
    color: colors.textLight
  },
  error: {
    ...typography.caption,
    color: colors.error,
    marginTop: spacing.xs
  }
});

export default PriceRangeSlider;
