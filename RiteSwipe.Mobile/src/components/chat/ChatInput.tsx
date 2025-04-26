import React, { useState, useRef } from 'react';
import {
  View,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  Keyboard,
  Animated,
  Platform
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, spacing, typography, borderRadius } from '@/theme';

interface ChatInputProps {
  onSend: (message: string) => void;
  onTyping: (isTyping: boolean) => void;
}

const ChatInput: React.FC<ChatInputProps> = ({ onSend, onTyping }) => {
  const [message, setMessage] = useState('');
  const [isRecording, setIsRecording] = useState(false);
  const recordingAnimation = useRef(new Animated.Value(0)).current;
  const inputRef = useRef<TextInput>(null);

  const handleSend = () => {
    if (message.trim()) {
      onSend(message.trim());
      setMessage('');
      Keyboard.dismiss();
    }
  };

  const handleTextChange = (text: string) => {
    setMessage(text);
    onTyping(text.length > 0);
  };

  const startRecording = () => {
    setIsRecording(true);
    Animated.sequence([
      Animated.timing(recordingAnimation, {
        toValue: 1,
        duration: 200,
        useNativeDriver: true
      }),
      Animated.loop(
        Animated.sequence([
          Animated.timing(recordingAnimation, {
            toValue: 1.2,
            duration: 600,
            useNativeDriver: true
          }),
          Animated.timing(recordingAnimation, {
            toValue: 1,
            duration: 600,
            useNativeDriver: true
          })
        ])
      )
    ]).start();
  };

  const stopRecording = () => {
    setIsRecording(false);
    recordingAnimation.setValue(0);
  };

  return (
    <View style={styles.container}>
      <TouchableOpacity
        style={styles.button}
        onPress={() => inputRef.current?.focus()}
      >
        <Icon name="emoticon-outline" size={24} color={colors.primary} />
      </TouchableOpacity>

      <TouchableOpacity style={styles.button}>
        <Icon name="paperclip" size={24} color={colors.primary} />
      </TouchableOpacity>

      <View style={styles.inputContainer}>
        <TextInput
          ref={inputRef}
          style={styles.input}
          value={message}
          onChangeText={handleTextChange}
          placeholder="Type a message..."
          placeholderTextColor={colors.textLight}
          multiline
          maxLength={1000}
        />
      </View>

      {message ? (
        <TouchableOpacity
          style={[styles.button, styles.sendButton]}
          onPress={handleSend}
        >
          <Icon name="send" size={24} color={colors.white} />
        </TouchableOpacity>
      ) : (
        <TouchableOpacity
          style={styles.button}
          onPressIn={startRecording}
          onPressOut={stopRecording}
        >
          <Animated.View
            style={[
              styles.micContainer,
              {
                transform: [
                  {
                    scale: recordingAnimation
                  }
                ]
              }
            ]}
          >
            <Icon
              name="microphone"
              size={24}
              color={isRecording ? colors.error : colors.primary}
            />
          </Animated.View>
        </TouchableOpacity>
      )}
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    paddingHorizontal: spacing.md,
    paddingTop: spacing.sm,
    paddingBottom: Platform.OS === 'ios' ? spacing.xl : spacing.md,
    backgroundColor: colors.white
  },
  button: {
    width: 40,
    height: 40,
    justifyContent: 'center',
    alignItems: 'center',
    borderRadius: borderRadius.round
  },
  inputContainer: {
    flex: 1,
    marginHorizontal: spacing.sm,
    backgroundColor: colors.background,
    borderRadius: borderRadius.xl,
    paddingHorizontal: spacing.md,
    paddingVertical: Platform.OS === 'ios' ? spacing.xs : 0,
    maxHeight: 100
  },
  input: {
    ...typography.body1,
    color: colors.text,
    padding: 0,
    minHeight: 40
  },
  sendButton: {
    backgroundColor: colors.primary,
    transform: [{ rotate: '0deg' }]
  },
  micContainer: {
    width: 40,
    height: 40,
    justifyContent: 'center',
    alignItems: 'center'
  }
});

export default ChatInput;
