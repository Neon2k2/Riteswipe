export const colors = {
  primary: '#FF6B6B',
  secondary: '#4ECDC4',
  background: '#F7F7F7',
  white: '#FFFFFF',
  black: '#000000',
  text: '#333333',
  textLight: '#666666',
  border: '#E1E1E1',
  error: '#FF5252',
  success: '#4CAF50',
  warning: '#FFC107',
  info: '#2196F3',
  overlay: 'rgba(0, 0, 0, 0.5)',
  transparent: 'transparent'
};

export const spacing = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  xxl: 40
};

export const typography = {
  heading1: {
    fontSize: 32,
    fontWeight: '700',
    lineHeight: 40
  },
  heading2: {
    fontSize: 24,
    fontWeight: '600',
    lineHeight: 32
  },
  heading3: {
    fontSize: 20,
    fontWeight: '600',
    lineHeight: 28
  },
  body1: {
    fontSize: 16,
    fontWeight: '400',
    lineHeight: 24
  },
  body2: {
    fontSize: 14,
    fontWeight: '400',
    lineHeight: 20
  },
  caption: {
    fontSize: 12,
    fontWeight: '400',
    lineHeight: 16
  }
};

export const shadows = {
  small: {
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 2,
  },
  medium: {
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 4,
    },
    shadowOpacity: 0.30,
    shadowRadius: 4.65,
    elevation: 4,
  },
  large: {
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 6,
    },
    shadowOpacity: 0.37,
    shadowRadius: 7.49,
    elevation: 8,
  }
};

export const borderRadius = {
  xs: 4,
  sm: 8,
  md: 12,
  lg: 16,
  xl: 24,
  round: 999
};

export const animations = {
  default: 300,
  fast: 200,
  slow: 500
};

export default {
  colors,
  spacing,
  typography,
  shadows,
  borderRadius,
  animations
};
