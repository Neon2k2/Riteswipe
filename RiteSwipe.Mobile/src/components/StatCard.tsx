import React from 'react';
import { View, Text, StyleSheet } from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, shadows, borderRadius } from '@/theme';
import { formatCurrency } from '@/utils/formatters';

interface StatCardProps {
  icon: string;
  value: number | string;
  label: string;
  isCurrency?: boolean;
}

const StatCard: React.FC<StatCardProps> = ({
  icon,
  value,
  label,
  isCurrency = false
}) => {
  return (
    <View style={styles.container}>
      <View style={styles.iconContainer}>
        <Icon name={icon} size={24} color={colors.primary} />
      </View>
      <Text style={styles.value}>
        {isCurrency ? formatCurrency(Number(value)) : value}
      </Text>
      <Text style={styles.label}>{label}</Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    backgroundColor: colors.white,
    padding: spacing.md,
    borderRadius: borderRadius.lg,
    marginHorizontal: spacing.xs,
    ...shadows.medium
  },
  iconContainer: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: colors.primary + '10',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: spacing.sm
  },
  value: {
    ...typography.heading2,
    color: colors.text,
    marginBottom: spacing.xs
  },
  label: {
    ...typography.caption,
    color: colors.textLight,
    textAlign: 'center'
  }
});

export default StatCard;
