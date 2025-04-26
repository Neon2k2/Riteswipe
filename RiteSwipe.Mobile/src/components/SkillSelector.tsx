import React, { useState } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Modal,
  FlatList
} from 'react-native';
import Icon from 'react-native-vector-icons/MaterialCommunityIcons';
import { useGetSkillsQuery, useSearchSkillsQuery } from '@/services/api';
import { colors, spacing, typography, borderRadius } from '@/theme';
import TextInput from './TextInput';
import Button from './Button';

interface SkillSelectorProps {
  selectedSkills: string[];
  onSkillsChange: (skills: string[]) => void;
  error?: string;
}

const SkillSelector: React.FC<SkillSelectorProps> = ({
  selectedSkills,
  onSkillsChange,
  error
}) => {
  const [modalVisible, setModalVisible] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');
  const { data: allSkills } = useGetSkillsQuery({});
  const { data: searchResults } = useSearchSkillsQuery(searchTerm, {
    skip: !searchTerm
  });

  const skills = searchTerm ? searchResults : allSkills;

  const toggleSkill = (skill: string) => {
    if (selectedSkills.includes(skill)) {
      onSkillsChange(selectedSkills.filter((s) => s !== skill));
    } else {
      onSkillsChange([...selectedSkills, skill]);
    }
  };

  const renderSkillItem = ({ item }: { item: string }) => (
    <TouchableOpacity
      style={[
        styles.skillItem,
        selectedSkills.includes(item) && styles.selectedSkillItem
      ]}
      onPress={() => toggleSkill(item)}
    >
      <Text
        style={[
          styles.skillText,
          selectedSkills.includes(item) && styles.selectedSkillText
        ]}
      >
        {item}
      </Text>
      {selectedSkills.includes(item) && (
        <Icon name="check" size={16} color={colors.white} />
      )}
    </TouchableOpacity>
  );

  return (
    <View style={styles.container}>
      <Text style={styles.label}>Required Skills</Text>
      
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        style={styles.selectedSkillsContainer}
        contentContainerStyle={styles.selectedSkillsContent}
      >
        {selectedSkills.map((skill) => (
          <View key={skill} style={styles.selectedSkillChip}>
            <Text style={styles.selectedSkillChipText}>{skill}</Text>
            <TouchableOpacity
              onPress={() => toggleSkill(skill)}
              hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
            >
              <Icon name="close" size={16} color={colors.white} />
            </TouchableOpacity>
          </View>
        ))}
        
        <Button
          icon="plus"
          variant="outline"
          size="small"
          onPress={() => setModalVisible(true)}
          style={styles.addButton}
        />
      </ScrollView>

      {error && <Text style={styles.error}>{error}</Text>}

      <Modal
        visible={modalVisible}
        animationType="slide"
        transparent
        onRequestClose={() => setModalVisible(false)}
      >
        <View style={styles.modalContainer}>
          <View style={styles.modalContent}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Select Skills</Text>
              <TouchableOpacity
                onPress={() => setModalVisible(false)}
                hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
              >
                <Icon name="close" size={24} color={colors.text} />
              </TouchableOpacity>
            </View>

            <TextInput
              placeholder="Search skills..."
              value={searchTerm}
              onChangeText={setSearchTerm}
              icon="magnify"
              containerStyle={styles.searchInput}
            />

            <FlatList
              data={skills}
              renderItem={renderSkillItem}
              keyExtractor={(item) => item}
              numColumns={2}
              showsVerticalScrollIndicator={false}
              contentContainerStyle={styles.skillsList}
            />

            <Button
              title="Done"
              onPress={() => setModalVisible(false)}
              style={styles.doneButton}
            />
          </View>
        </View>
      </Modal>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    marginBottom: spacing.md
  },
  label: {
    ...typography.body1,
    color: colors.text,
    marginBottom: spacing.sm
  },
  selectedSkillsContainer: {
    maxHeight: 50
  },
  selectedSkillsContent: {
    gap: spacing.sm
  },
  selectedSkillChip: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: colors.primary,
    paddingVertical: spacing.xs,
    paddingHorizontal: spacing.sm,
    borderRadius: borderRadius.round,
    gap: spacing.xs
  },
  selectedSkillChipText: {
    ...typography.body2,
    color: colors.white
  },
  addButton: {
    height: 32,
    width: 32,
    borderRadius: 16,
    padding: 0
  },
  error: {
    ...typography.caption,
    color: colors.error,
    marginTop: spacing.xs
  },
  modalContainer: {
    flex: 1,
    backgroundColor: colors.overlay,
    justifyContent: 'flex-end'
  },
  modalContent: {
    backgroundColor: colors.background,
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    padding: spacing.lg,
    maxHeight: '80%'
  },
  modalHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: spacing.md
  },
  modalTitle: {
    ...typography.heading2
  },
  searchInput: {
    marginBottom: spacing.md
  },
  skillsList: {
    gap: spacing.sm,
    paddingBottom: spacing.xl
  },
  skillItem: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    backgroundColor: colors.white,
    padding: spacing.sm,
    marginHorizontal: spacing.xs,
    borderRadius: borderRadius.md,
    borderWidth: 1,
    borderColor: colors.border
  },
  selectedSkillItem: {
    backgroundColor: colors.primary,
    borderColor: colors.primary
  },
  skillText: {
    ...typography.body2,
    color: colors.text
  },
  selectedSkillText: {
    color: colors.white
  },
  doneButton: {
    marginTop: spacing.md
  }
});

export default SkillSelector;
