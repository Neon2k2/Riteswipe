import React from 'react';
import { View, Text, StyleSheet, Pressable } from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, borderRadius } from '@/theme';

interface DurationPickerProps {
  value: number;
  onValueChange: (value: number) => void;
  error?: string;
}

const durations = [
  { value: 1, label: '1 day', icon: 'clock-fast' },
  { value: 3, label: '3 days', icon: 'clock' },
  { value: 7, label: '1 week', icon: 'calendar-week' },
  { value: 14, label: '2 weeks', icon: 'calendar' },
  { value: 30, label: '1 month', icon: 'calendar-month' }
];

const DurationPicker: React.FC<DurationPickerProps> = ({
  value,
  onValueChange,
  error
}) => {
  return (
    <View style={styles.container}>
      <Text style={styles.label}>Task Duration</Text>

      <View style={styles.optionsContainer}>
        {durations.map((duration) => (
          <Pressable
            key={duration.value}
            style={[
              styles.option,
              value === duration.value && styles.selectedOption
            ]}
            onPress={() => onValueChange(duration.value)}
          >
            <Icon
              name={duration.icon}
              size={24}
              color={value === duration.value ? colors.white : colors.primary}
            />
            <Text
              style={[
                styles.optionText,
                value === duration.value && styles.selectedOptionText
              ]}
            >
              {duration.label}
            </Text>
          </Pressable>
        ))}
      </View>

      {error && <Text style={styles.error}>{error}</Text>}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginBottom: spacing.md
  },
  label: {
    ...typography.body1,
    color: colors.text,
    marginBottom: spacing.sm
  },
  optionsContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm
  },
  option: {
    flex: 1,
    minWidth: '45%',
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    backgroundColor: colors.white,
    padding: spacing.sm,
    borderRadius: borderRadius.md,
    borderWidth: 1,
    borderColor: colors.border
  },
  selectedOption: {
    backgroundColor: colors.primary,
    borderColor: colors.primary
  },
  optionText: {
    ...typography.body2,
    color: colors.text
  },
  selectedOptionText: {
    color: colors.white
  },
  error: {
    ...typography.caption,
    color: colors.error,
    marginTop: spacing.xs
  }
});

export default DurationPicker;
