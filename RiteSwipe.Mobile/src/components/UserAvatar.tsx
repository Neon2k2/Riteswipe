import React from 'react';
import {
  View,
  Image,
  StyleSheet,
  StyleProp,
  ViewStyle,
  TouchableOpacity
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { colors, borderRadius } from '@/theme';

interface UserAvatarProps {
  uri?: string | null;
  size?: number;
  style?: StyleProp<ViewStyle>;
  onPress?: () => void;
  editable?: boolean;
}

const UserAvatar: React.FC<UserAvatarProps> = ({
  uri,
  size = 40,
  style,
  onPress,
  editable = false
}) => {
  const Container = onPress ? TouchableOpacity : View;

  return (
    <Container
      style={[
        styles.container,
        {
          width: size,
          height: size,
          borderRadius: size / 2
        },
        style
      ]}
      onPress={onPress}
    >
      {uri ? (
        <Image
          source={{ uri }}
          style={[
            styles.image,
            {
              width: size,
              height: size,
              borderRadius: size / 2
            }
          ]}
        />
      ) : (
        <Icon
          name="account"
          size={size * 0.6}
          color={colors.white}
        />
      )}
      
      {editable && (
        <View style={styles.editBadge}>
          <Icon name="pencil" size={12} color={colors.white} />
        </View>
      )}
    </Container>
  );
};

const styles = StyleSheet.create({
  container: {
    backgroundColor: colors.primary,
    justifyContent: 'center',
    alignItems: 'center',
    overflow: 'hidden'
  },
  image: {
    resizeMode: 'cover'
  },
  editBadge: {
    position: 'absolute',
    right: 0,
    bottom: 0,
    backgroundColor: colors.primary,
    width: 20,
    height: 20,
    borderRadius: borderRadius.round,
    justifyContent: 'center',
    alignItems: 'center',
    borderWidth: 2,
    borderColor: colors.white
  }
});

export default UserAvatar;
