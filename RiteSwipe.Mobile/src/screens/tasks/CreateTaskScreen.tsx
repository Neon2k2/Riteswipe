import React, { useState } from 'react';
import {
  View,
  StyleSheet,
  ScrollView,
  KeyboardAvoidingView,
  Platform,
  Image,
  Text,
  TextInput,
  TouchableOpacity
} from 'react-native';
import { useNavigation } from '@react-navigation/native';
import { Formik, FormikHelpers, FormikProps } from 'formik';
import * as Yup from 'yup';
import { launchImageLibrary, Asset } from 'react-native-image-picker';

import { useCreateTaskMutation } from '@/services/api';
import { colors, spacing, typography, shadows } from '@/theme';
import TextInputComponent from '@/components/TextInput';
import Button from '@/components/Button';
import ErrorMessage from '@/components/ErrorMessage';
import SkillSelector from '@/components/SkillSelector';
import PriceRangeSlider from '@/components/PriceRangeSlider';
import DurationPicker from '@/components/DurationPicker';
import ConfettiAnimation from '@/components/ConfettiAnimation';

import type { Task } from '../../types';

interface TaskFormValues {
  title: string;
  description: string;
  budget: string;
  skills: string[];
  images: string[];
}

const validationSchema = Yup.object().shape({
  title: Yup.string().required('Title is required'),
  description: Yup.string().required('Description is required'),
  budget: Yup.number().required('Budget is required').min(0, 'Budget must be positive'),
  skills: Yup.array().of(Yup.string()).min(1, 'At least one skill is required')
});

const initialValues: TaskFormValues = {
  title: '',
  description: '',
  budget: '',
  skills: [],
  images: []
};

const CreateTaskScreen: React.FC = () => {
  const navigation = useNavigation();
  const [createTask] = useCreateTaskMutation();
  const [showConfetti, setShowConfetti] = useState(false);

  const handleImagePicker = async (setFieldValue: (field: string, value: any) => void): Promise<void> => {
    try {
      const result = await launchImageLibrary({
        mediaType: 'photo',
        quality: 0.8,
        selectionLimit: 5
      });

      if (result.assets) {
        setFieldValue('images', result.assets.map((asset: Asset) => asset.uri || ''));
      }
    } catch (error) {
      console.error('Error picking image:', error);
    }
  };

  const handleSubmit = async (values: TaskFormValues, { setSubmitting }: FormikHelpers<TaskFormValues>): Promise<void> => {
    try {
      await createTask({
        title: values.title,
        description: values.description,
        budget: parseFloat(values.budget),
        skills: values.skills,
        images: values.images
      }).unwrap();

      setShowConfetti(true);
      setTimeout(() => {
        navigation.goBack();
      }, 2000);
    } catch (error) {
      console.error('Error creating task:', error);
    } finally {
      setSubmitting(false);
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
        <Formik<TaskFormValues>
          initialValues={initialValues}
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
          }: FormikProps<TaskFormValues>) => (
            <View style={styles.form}>
              <View style={styles.imageSection}>
                {values.images.length > 0 ? (
                  <Image
                    source={{ uri: values.images[0] }}
                    style={styles.selectedImage}
                  />
                ) : (
                  <TouchableOpacity
                    style={styles.imageButton}
                    onPress={() => handleImagePicker(setFieldValue)}
                  >
                    <Text style={styles.imageButtonText}>Add Task Image</Text>
                  </TouchableOpacity>
                )}
              </View>

              <View style={styles.inputContainer}>
                <Text style={styles.label}>Task Title</Text>
                <TextInputComponent
                  style={styles.input}
                  onChangeText={handleChange('title')}
                  onBlur={handleBlur('title')}
                  value={values.title}
                  placeholder="What needs to be done?"
                  icon="format-title"
                />
                {touched.title && errors.title && (
                  <Text style={styles.errorText}>{errors.title}</Text>
                )}
              </View>

              <View style={styles.inputContainer}>
                <Text style={styles.label}>Description</Text>
                <TextInputComponent
                  style={[styles.input, styles.textArea]}
                  onChangeText={handleChange('description')}
                  onBlur={handleBlur('description')}
                  value={values.description}
                  placeholder="Describe your task in detail"
                  multiline
                  numberOfLines={4}
                  icon="text"
                />
                {touched.description && errors.description && (
                  <Text style={styles.errorText}>{errors.description}</Text>
                )}
              </View>

              <View style={styles.inputContainer}>
                <Text style={styles.label}>Budget</Text>
                <TextInputComponent
                  style={styles.input}
                  onChangeText={(value: string) => {
                    const numericValue = value.replace(/[^0-9.]/g, '');
                    handleChange('budget')(numericValue);
                  }}
                  onBlur={handleBlur('budget')}
                  value={values.budget}
                  placeholder="Enter budget"
                  keyboardType="numeric"
                />
                {touched.budget && errors.budget && (
                  <Text style={styles.errorText}>{errors.budget}</Text>
                )}
              </View>

              <View style={styles.inputContainer}>
                <Text style={styles.label}>Skills</Text>
                <TextInputComponent
                  style={styles.input}
                  onChangeText={(value: string) => {
                    const skillsArray = value.split(',').map(s => s.trim());
                    setFieldValue('skills', skillsArray);
                  }}
                  value={values.skills.join(', ')}
                  placeholder="Enter skills (comma-separated)"
                />
                {touched.skills && errors.skills && (
                  <Text style={styles.errorText}>{errors.skills}</Text>
                )}
              </View>

              <Button
                title="Create Task"
                onPress={handleSubmit}
                icon="check-circle"
                style={styles.submitButton}
              />
            </View>
          )}
        </Formik>
      </ScrollView>

      {showConfetti && <ConfettiAnimation />}
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
  imageSection: {
    height: 200,
    borderRadius: 16,
    overflow: 'hidden',
    marginBottom: spacing.md,
    ...shadows.medium
  },
  imageButton: {
    height: '100%',
    backgroundColor: colors.white,
    justifyContent: 'center',
    alignItems: 'center'
  },
  imageButtonText: {
    fontSize: 16,
    color: colors.text
  },
  selectedImage: {
    width: '100%',
    height: '100%',
    resizeMode: 'cover'
  },
  inputContainer: {
    marginBottom: 16
  },
  label: {
    fontSize: 16,
    fontWeight: '600',
    marginBottom: 8
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    padding: 12,
    fontSize: 16
  },
  textArea: {
    height: 120,
    textAlignVertical: 'top'
  },
  errorText: {
    color: 'red',
    fontSize: 12,
    marginTop: 4
  },
  submitButton: {
    marginTop: spacing.lg,
    marginBottom: spacing.xl
  }
});

export default CreateTaskScreen;
