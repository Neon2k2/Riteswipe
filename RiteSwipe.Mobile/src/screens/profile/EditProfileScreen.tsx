import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
  Alert
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Formik } from 'formik';
import * as Yup from 'yup';
import { launchImageLibrary } from 'react-native-image-picker';
import { useDispatch, useSelector } from 'react-redux';

import { useUpdateUserProfileMutation } from '@/services/api';
import { updateUser } from '@/store/slices/authSlice';
import { RootState } from '@/store';
import { colors, spacing } from '@/theme';
import TextInput from '@/components/TextInput';
import Button from '@/components/Button';
import UserAvatar from '@/components/UserAvatar';
import SkillSelector from '@/components/SkillSelector';

const validationSchema = Yup.object().shape({
  name: Yup.string().required('Name is required'),
  tagline: Yup.string().required('Tagline is required'),
  bio: Yup.string().required('Bio is required'),
  location: Yup.string().required('Location is required'),
  skills: Yup.array().min(1, 'At least one skill is required')
});

const EditProfileScreen = () => {
  const navigation = useNavigation();
  const dispatch = useDispatch();
  const user = useSelector((state: RootState) => state.auth.user);
  const [updateProfile, { isLoading }] = useUpdateUserProfileMutation();
  const [avatarUri, setAvatarUri] = useState<string | null>(user?.avatarUrl || null);

  const handleImagePicker = async () => {
    try {
      const result = await launchImageLibrary({
        mediaType: 'photo',
        quality: 0.8,
        maxWidth: 1000,
        maxHeight: 1000
      });

      if (result.assets?.[0]?.uri) {
        setAvatarUri(result.assets[0].uri);
      }
    } catch (error) {
      Alert.alert('Error', 'Failed to pick image');
    }
  };

  const handleSubmit = async (values: any) => {
    try {
      const updatedProfile = await updateProfile({
        ...values,
        avatarUrl: avatarUri
      }).unwrap();

      dispatch(updateUser(updatedProfile));
      navigation.goBack();
    } catch (error) {
      Alert.alert('Error', 'Failed to update profile');
    }
  };

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : 'height'}
      style={styles.container}
    >
      <ScrollView
        contentContainerStyle={styles.scrollContent}
        showsVerticalScrollIndicator={false}
      >
        <Formik
          initialValues={{
            name: user?.name || '',
            tagline: user?.tagline || '',
            bio: user?.bio || '',
            location: user?.location || '',
            skills: user?.skills || []
          }}
          validationSchema={validationSchema}
          onSubmit={handleSubmit}
        >
          {({
            handleChange,
            handleBlur,
            handleSubmit,
            values,
            errors,
            touched,
            setFieldValue
          }) => (
            <View style={styles.form}>
              <View style={styles.avatarContainer}>
                <UserAvatar
                  uri={avatarUri}
                  size={120}
                  onPress={handleImagePicker}
                  editable
                />
              </View>

              <TextInput
                label="Full Name"
                value={values.name}
                onChangeText={handleChange('name')}
                onBlur={handleBlur('name')}
                error={touched.name ? errors.name : undefined}
                icon="account"
              />

              <TextInput
                label="Tagline"
                value={values.tagline}
                onChangeText={handleChange('tagline')}
                onBlur={handleBlur('tagline')}
                error={touched.tagline ? errors.tagline : undefined}
                icon="tag"
                placeholder="A brief professional headline"
              />

              <TextInput
                label="Bio"
                value={values.bio}
                onChangeText={handleChange('bio')}
                onBlur={handleBlur('bio')}
                error={touched.bio ? errors.bio : undefined}
                icon="text"
                multiline
                numberOfLines={4}
                placeholder="Tell us about yourself"
              />

              <TextInput
                label="Location"
                value={values.location}
                onChangeText={handleChange('location')}
                onBlur={handleBlur('location')}
                error={touched.location ? errors.location : undefined}
                icon="map-marker"
                placeholder="Your city, country"
              />

              <SkillSelector
                selectedSkills={values.skills}
                onSkillsChange={(skills) => setFieldValue('skills', skills)}
                error={touched.skills ? errors.skills : undefined}
              />

              <View style={styles.buttonContainer}>
                <Button
                  title="Cancel"
                  variant="outline"
                  onPress={() => navigation.goBack()}
                  style={styles.button}
                />
                <Button
                  title="Save Changes"
                  onPress={handleSubmit}
                  loading={isLoading}
                  style={styles.button}
                />
              </View>
            </View>
          )}
        </Formik>
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
  form: {
    gap: spacing.md
  },
  avatarContainer: {
    alignItems: 'center',
    marginBottom: spacing.lg
  },
  buttonContainer: {
    flexDirection: 'row',
    gap: spacing.md,
    marginTop: spacing.lg,
    marginBottom: spacing.xl
  },
  button: {
    flex: 1
  }
});

export default EditProfileScreen;
