import React from 'react';
import {
  View,
  StyleSheet,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  Text,
  Image
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { useDispatch } from 'react-redux';

import { useLoginMutation } from '@/services/api';
import { setCredentials } from '@/store/slices/authSlice';
import { colors, spacing, typography } from '@/theme';
import TextInput from '@/components/TextInput';
import Button from '@/components/Button';
import ErrorMessage from '@/components/ErrorMessage';

const validationSchema = Yup.object().shape({
  email: Yup.string()
    .email('Invalid email')
    .required('Email is required'),
  password: Yup.string()
    .min(6, 'Password must be at least 6 characters')
    .required('Password is required')
});

const LoginScreen = () => {
  const navigation = useNavigation();
  const dispatch = useDispatch();
  const [login, { isLoading, error }] = useLoginMutation();

  const handleLogin = async (values: { email: string; password: string }) => {
    try {
      const result = await login(values).unwrap();
      dispatch(setCredentials(result));
    } catch (err) {
      console.error('Failed to login:', err);
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}
    >
      <ScrollView
        contentContainerStyle={styles.scrollContent}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.header}>
          <Image
            source={require('@/assets/images/logo.png')}
            style={styles.logo}
            resizeMode="contain"
          />
          <Text style={styles.title}>Welcome Back!</Text>
          <Text style={styles.subtitle}>
            Sign in to continue finding and posting tasks
          </Text>
        </View>

        <Formik
          initialValues={{ email: '', password: '' }}
          validationSchema={validationSchema}
          onSubmit={handleLogin}
        >
          {({
            handleChange,
            handleBlur,
            handleSubmit,
            values,
            errors,
            touched
          }) => (
            <View style={styles.form}>
              <TextInput
                label="Email"
                value={values.email}
                onChangeText={handleChange('email')}
                onBlur={handleBlur('email')}
                error={touched.email ? errors.email : undefined}
                keyboardType="email-address"
                autoCapitalize="none"
                autoComplete="email"
                icon="email"
              />

              <TextInput
                label="Password"
                value={values.password}
                onChangeText={handleChange('password')}
                onBlur={handleBlur('password')}
                error={touched.password ? errors.password : undefined}
                secureTextEntry
                icon="lock"
              />

              {error && (
                <ErrorMessage message="Invalid email or password" />
              )}

              <Button
                title="Sign In"
                onPress={handleSubmit}
                loading={isLoading}
                style={styles.button}
              />
            </View>
          )}
        </Formik>

        <View style={styles.footer}>
          <Text style={styles.footerText}>Don't have an account?</Text>
          <Button
            title="Sign Up"
            variant="text"
            onPress={() => navigation.navigate('Register')}
            style={styles.registerButton}
          />
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background
  },
  scrollContent: {
    flexGrow: 1,
    padding: spacing.lg
  },
  header: {
    alignItems: 'center',
    marginVertical: spacing.xl
  },
  logo: {
    width: 120,
    height: 120,
    marginBottom: spacing.md
  },
  title: {
    ...typography.heading1,
    color: colors.primary,
    marginBottom: spacing.xs
  },
  subtitle: {
    ...typography.body1,
    color: colors.textLight,
    textAlign: 'center'
  },
  form: {
    gap: spacing.md
  },
  button: {
    marginTop: spacing.md
  },
  footer: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    marginTop: spacing.xl
  },
  footerText: {
    ...typography.body2,
    color: colors.textLight
  },
  registerButton: {
    marginLeft: spacing.xs
  }
});

export default LoginScreen;
