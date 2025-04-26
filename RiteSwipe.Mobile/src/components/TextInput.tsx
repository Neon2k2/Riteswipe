import React, { useState } from 'react';
import {
  View,
  TextInput as RNTextInput,
  Text,
  StyleSheet,
  TextInputProps as RNTextInputProps,
  StyleProp,
  ViewStyle,
  Pressable
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, borderRadius } from '@/theme';

interface TextInputProps extends RNTextInputProps {
  label?: string;
  error?: string;
  icon?: string;
  rightIcon?: string;
  onRightIconPress?: () => void;
  containerStyle?: StyleProp<ViewStyle>;
}

const TextInput: React.FC<TextInputProps> = ({
  label,
  error,
  icon,
  rightIcon,
  onRightIconPress,
  containerStyle,
  secureTextEntry,
  ...rest
}) => {
  const [isFocused, setIsFocused] = useState(false);
  const [isPasswordVisible, setIsPasswordVisible] = useState(!secureTextEntry);

  const togglePasswordVisibility = () => {
    setIsPasswordVisible(!isPasswordVisible);
  };

  return (
    <View style={[styles.container, containerStyle]}>
      {label && <Text style={styles.label}>{label}</Text>}
      
      <View
        style={[
          styles.inputContainer,
          isFocused && styles.focusedContainer,
          error && styles.errorContainer
        ]}
      >
        {icon && (
          <Icon
            name={icon}
            size={20}
            color={error ? colors.error : isFocused ? colors.primary : colors.textLight}
            style={styles.leftIcon}
          />
        )}

        <RNTextInput
          style={[
            styles.input,
            icon && styles.inputWithLeftIcon,
            (rightIcon || secureTextEntry) && styles.inputWithRightIcon
          ]}
          placeholderTextColor={colors.textLight}
          onFocus={() => setIsFocused(true)}
          onBlur={() => setIsFocused(false)}
          secureTextEntry={secureTextEntry && !isPasswordVisible}
          {...rest}
        />

        {secureTextEntry && (
          <Pressable
            onPress={togglePasswordVisibility}
            style={styles.rightIconContainer}
          >
            <Icon
              name={isPasswordVisible ? 'eye-off' : 'eye'}
              size={20}
              color={colors.textLight}
            />
          </Pressable>
        )}

        {rightIcon && !secureTextEntry && (
          <Pressable
            onPress={onRightIconPress}
            style={styles.rightIconContainer}
          >
            <Icon
              name={rightIcon}
              size={20}
              color={colors.textLight}
            />
          </Pressable>
        )}
      </View>

      {error && <Text style={styles.errorText}>{error}</Text>}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginBottom: spacing.sm
  },
  label: {
    ...typography.body2,
    color: colors.text,
    marginBottom: spacing.xs
  },
  inputContainer: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderColor: colors.border,
    borderRadius: borderRadius.md,
    backgroundColor: colors.white,
    minHeight: 48
  },
  focusedContainer: {
    borderColor: colors.primary
  },
  errorContainer: {
    borderColor: colors.error
  },
  input: {
    flex: 1,
    ...typography.body1,
    color: colors.text,
    paddingVertical: spacing.sm,
    paddingHorizontal: spacing.md
  },
  inputWithLeftIcon: {
    paddingLeft: 0
  },
  inputWithRightIcon: {
    paddingRight: 0
  },
  leftIcon: {
    marginLeft: spacing.md
  },
  rightIconContainer: {
    padding: spacing.md
  },
  errorText: {
    ...typography.caption,
    color: colors.error,
    marginTop: spacing.xs
  }
});

export default TextInput;
