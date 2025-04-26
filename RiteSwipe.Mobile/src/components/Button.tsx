import React from 'react';
import {
  TouchableOpacity,
  Text,
  StyleSheet,
  ActivityIndicator,
  ViewStyle,
  TextStyle,
  StyleProp
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, borderRadius } from '@/theme';

interface ButtonProps {
  onPress: () => void;
  title?: string;
  icon?: string;
  variant?: 'primary' | 'secondary' | 'outline' | 'text';
  size?: 'small' | 'medium' | 'large';
  disabled?: boolean;
  loading?: boolean;
  color?: string;
  style?: StyleProp<ViewStyle>;
  textStyle?: StyleProp<TextStyle>;
}

const Button: React.FC<ButtonProps> = ({
  onPress,
  title,
  icon,
  variant = 'primary',
  size = 'medium',
  disabled = false,
  loading = false,
  color,
  style,
  textStyle
}) => {
  const getContainerStyle = () => {
    const baseStyle: ViewStyle = {
      borderRadius: borderRadius.md,
      flexDirection: 'row',
      alignItems: 'center',
      justifyContent: 'center'
    };

    // Size variations
    switch (size) {
      case 'small':
        baseStyle.paddingVertical = spacing.xs;
        baseStyle.paddingHorizontal = spacing.sm;
        break;
      case 'large':
        baseStyle.paddingVertical = spacing.md;
        baseStyle.paddingHorizontal = spacing.lg;
        break;
      default:
        baseStyle.paddingVertical = spacing.sm;
        baseStyle.paddingHorizontal = spacing.md;
    }

    // Variant styles
    switch (variant) {
      case 'secondary':
        baseStyle.backgroundColor = colors.secondary;
        break;
      case 'outline':
        baseStyle.backgroundColor = colors.transparent;
        baseStyle.borderWidth = 1;
        baseStyle.borderColor = color || colors.primary;
        break;
      case 'text':
        baseStyle.backgroundColor = colors.transparent;
        break;
      default:
        baseStyle.backgroundColor = color || colors.primary;
    }

    if (disabled) {
      baseStyle.opacity = 0.5;
    }

    return baseStyle;
  };

  const getTextStyle = () => {
    const baseStyle: TextStyle = {
      ...typography.body1,
      textAlign: 'center'
    };

    // Size variations
    switch (size) {
      case 'small':
        baseStyle.fontSize = 14;
        break;
      case 'large':
        baseStyle.fontSize = 18;
        break;
    }

    // Variant styles
    switch (variant) {
      case 'outline':
      case 'text':
        baseStyle.color = color || colors.primary;
        break;
      default:
        baseStyle.color = colors.white;
    }

    return baseStyle;
  };

  const iconSize = size === 'small' ? 16 : size === 'large' ? 24 : 20;
  const iconColor = variant === 'outline' || variant === 'text' 
    ? (color || colors.primary) 
    : colors.white;

  return (
    <TouchableOpacity
      onPress={onPress}
      disabled={disabled || loading}
      style={[styles.container, getContainerStyle(), style]}
      activeOpacity={0.8}
    >
      {loading ? (
        <ActivityIndicator 
          color={variant === 'outline' ? (color || colors.primary) : colors.white} 
          size="small" 
        />
      ) : (
        <>
          {icon && (
            <Icon
              name={icon}
              size={iconSize}
              color={iconColor}
              style={title ? styles.iconWithText : undefined}
            />
          )}
          {title && (
            <Text style={[getTextStyle(), textStyle]}>
              {title}
            </Text>
          )}
        </>
      )}
    </TouchableOpacity>
  );
};

const styles = StyleSheet.create({
  container: {
    minWidth: 64,
  },
  iconWithText: {
    marginRight: spacing.xs
  }
});

export default Button;
