import React, { useState, useEffect, useRef } from 'react';
import {
  View,
  StyleSheet,
  KeyboardAvoidingView,
  Platform,
  FlatList,
  Keyboard,
  Animated
} from 'react-native';
import { useRoute, useNavigation } from '@react-navigation/native';
import { useSelector } from 'react-redux';

import { signalRService } from '@/services/signalR';
import { RootState } from '@/store';
import { colors, spacing } from '@/theme';
import ChatHeader from '@/components/chat/ChatHeader';
import MessageBubble from '@/components/chat/MessageBubble';
import ChatInput from '@/components/chat/ChatInput';
import TypingIndicator from '@/components/chat/TypingIndicator';

interface Message {
  id: string;
  content: string;
  senderId: string;
  timestamp: string;
}

const ChatScreen = () => {
  const route = useRoute();
  const navigation = useNavigation();
  const { userId: recipientId } = route.params;
  const currentUser = useSelector((state: RootState) => state.auth.user);
  const [messages, setMessages] = useState<Message[]>([]);
  const [isTyping, setIsTyping] = useState(false);
  const [inputHeight, setInputHeight] = useState(0);
  const typingTimeoutRef = useRef<NodeJS.Timeout>();
  const keyboardHeight = useRef(new Animated.Value(0)).current;

  useEffect(() => {
    const keyboardWillShow = Keyboard.addListener(
      Platform.OS === 'ios' ? 'keyboardWillShow' : 'keyboardDidShow',
      (e) => {
        Animated.timing(keyboardHeight, {
          toValue: e.endCoordinates.height,
          duration: Platform.OS === 'ios' ? 250 : 0,
          useNativeDriver: false
        }).start();
      }
    );

    const keyboardWillHide = Keyboard.addListener(
      Platform.OS === 'ios' ? 'keyboardWillHide' : 'keyboardDidHide',
      () => {
        Animated.timing(keyboardHeight, {
          toValue: 0,
          duration: Platform.OS === 'ios' ? 250 : 0,
          useNativeDriver: false
        }).start();
      }
    );

    return () => {
      keyboardWillShow.remove();
      keyboardWillHide.remove();
    };
  }, []);

  useEffect(() => {
    signalRService.subscribeToMessages((message) => {
      setMessages((prev) => [...prev, message]);
    });

    signalRService.subscribeToTyping(({ userId, isTyping }) => {
      if (userId === recipientId) {
        setIsTyping(isTyping);
      }
    });

    return () => {
      signalRService.stopConnection();
    };
  }, [recipientId]);

  const handleSend = async (content: string) => {
    if (!content.trim()) return;

    const newMessage: Message = {
      id: Date.now().toString(),
      content,
      senderId: currentUser!.id,
      timestamp: new Date().toISOString()
    };

    setMessages((prev) => [...prev, newMessage]);
    await signalRService.sendMessage(recipientId, content);
  };

  const handleTyping = (isTyping: boolean) => {
    if (typingTimeoutRef.current) {
      clearTimeout(typingTimeoutRef.current);
    }

    signalRService.sendTypingStatus(recipientId, isTyping);

    if (isTyping) {
      typingTimeoutRef.current = setTimeout(() => {
        signalRService.sendTypingStatus(recipientId, false);
      }, 3000);
    }
  };

  const renderMessage = ({ item }: { item: Message }) => (
    <MessageBubble
      message={item}
      isOwnMessage={item.senderId === currentUser?.id}
    />
  );

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      style={styles.container}
      keyboardVerticalOffset={Platform.OS === 'ios' ? 90 : 0}
    >
      <ChatHeader
        recipientId={recipientId}
        onBack={() => navigation.goBack()}
      />

      <FlatList
        data={messages}
        renderItem={renderMessage}
        keyExtractor={(item) => item.id}
        contentContainerStyle={styles.messageList}
        inverted
        onLayout={(e) => setInputHeight(e.nativeEvent.layout.height)}
      />

      {isTyping && <TypingIndicator />}

      <Animated.View
        style={[
          styles.inputContainer,
          {
            paddingBottom: keyboardHeight.interpolate({
              inputRange: [0, 1],
              outputRange: [Platform.OS === 'ios' ? 34 : 0, 0]
            })
          }
        ]}
      >
        <ChatInput
          onSend={handleSend}
          onTyping={handleTyping}
        />
      </Animated.View>
    </KeyboardAvoidingView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: colors.background
  },
  messageList: {
    paddingHorizontal: spacing.md,
    paddingVertical: spacing.lg
  },
  inputContainer: {
    borderTopWidth: 1,
    borderTopColor: colors.border,
    backgroundColor: colors.white
  }
});

export default ChatScreen;
