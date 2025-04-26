import { useEffect } from 'react';
import { useSelector } from 'react-redux';
import { signalRService } from '../services/signalRService';
import { RootState } from '../store';

export const useSignalR = () => {
  const { token } = useSelector((state: RootState) => state.auth);

  useEffect(() => {
    const connectToSignalR = async () => {
      if (token) {
        try {
          await signalRService.startConnections(token);
        } catch (error) {
          console.error('Failed to connect to SignalR:', error);
        }
      }
    };

    connectToSignalR();

    return () => {
      signalRService.stopConnections();
    };
  }, [token]);

  return signalRService;
};
